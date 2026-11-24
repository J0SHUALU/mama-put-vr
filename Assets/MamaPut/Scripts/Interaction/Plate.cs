using System.Collections.Generic;
using UnityEngine;

namespace MamaPut
{
    public class Plate : GrabbableItem, IPlate, IServable, IPoolable
    {
        [Tooltip("Where portions sit on the plate. The plate holds as many portions as there are slots.")]
        [SerializeField] Transform[] slots;

        readonly PlateContents contents = new PlateContents();
        readonly List<GameObject> visuals = new List<GameObject>();
        Rigidbody body;

        public System.Action<Plate> ReturnToPool;

        public PlateContents Contents => contents;
        public int Price() => contents.Price();
        public bool HandedOver => grab != null && !grab.enabled;

        protected override void Awake()
        {
            base.Awake();
            body = GetComponent<Rigidbody>();
        }

        public bool AddPortion(DishData dish)
        {
            if (visuals.Count >= slots.Length) return false;
            contents.Add(dish);
            visuals.Add(PortionPool.Get(dish, slots[visuals.Count]));
            return true;
        }

        public void Clear()
        {
            foreach (var v in visuals) PortionPool.Release(v);
            visuals.Clear();
            contents.Clear();
        }

        public void HandTo(Transform hand)
        {
            ForceRelease();
            grab.enabled = false;
            if (body != null)
            {
                body.isKinematic = true;
                body.interpolation = RigidbodyInterpolation.None;
                body.detectCollisions = false;
            }
            transform.SetParent(hand, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            if (body != null)
            {
                body.position = transform.position;
                body.rotation = transform.rotation;
            }
        }

        public void Recycle()
        {
            if (ReturnToPool != null) ReturnToPool(this);
            else Destroy(gameObject);
        }

        public void OnSpawn()
        {
            grab.enabled = true;
            if (body != null)
            {
                body.isKinematic = false;
                body.interpolation = RigidbodyInterpolation.Interpolate;
                body.detectCollisions = true;
            }
        }

        public void OnDespawn()
        {
            Clear();
            transform.SetParent(null, true);
        }
    }
}
