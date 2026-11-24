using UnityEngine;

namespace MamaPut
{
    public class NairaNote : GrabbableItem, ICurrency, IPoolable
    {
        [SerializeField] int value = 100;

        Rigidbody body;

        public int Value => value;
        public System.Action<NairaNote> ReturnToPool;

        protected override void Awake()
        {
            base.Awake();
            body = GetComponent<Rigidbody>();
        }

        public void HoldStill()
        {
            if (body != null) body.isKinematic = true;
        }

        protected override void OnReleased()
        {
            if (body != null) StartCoroutine(DropNextFrame());
        }

        System.Collections.IEnumerator DropNextFrame()
        {
            yield return null;
            body.isKinematic = false;
        }

        public void Recycle()
        {
            ForceRelease();
            if (ReturnToPool != null) ReturnToPool(this);
            else Destroy(gameObject);
        }

        public void OnSpawn()
        {
            if (body == null) return;
            body.isKinematic = false;
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        public void OnDespawn() => transform.SetParent(null, true);
    }
}
