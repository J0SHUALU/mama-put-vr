using UnityEngine;

namespace MamaPut
{
    public abstract class Utensil : GrabbableItem, IUtensil
    {
        [SerializeField] Transform loadPoint;
        [Tooltip("How far the utensil has to tip (degrees from upright) before the food falls off.")]
        [SerializeField] float dropAngle = 70f;
        [SerializeField] AudioSource sfx;
        [SerializeField] AudioClip[] scoopClips;
        [SerializeField] AudioClip[] dropClips;

        DishData loadedDish;
        GameObject loadedVisual;
        IPlate plateBelow;

        public bool IsLoaded => loadedDish != null;

        public abstract bool CanServe(DishData dish);

        public bool Scoop(IFoodSource source)
        {
            if (IsLoaded || !CanServe(source.Dish) || !source.TakePortion()) return false;
            loadedDish = source.Dish;
            loadedVisual = PortionPool.Get(loadedDish, loadPoint);
            Pulse(0.4f, 0.08f);
            if (!PlayDishSound(loadedDish.scoopSound)) PlayRandom(scoopClips);
            return true;
        }

        public bool Drop(IPlate target)
        {
            if (!IsLoaded || !target.AddPortion(loadedDish)) return false;
            bool played = PlayDishSound(loadedDish.serveSound);
            PortionPool.Release(loadedVisual);
            loadedVisual = null;
            loadedDish = null;
            Pulse(0.3f, 0.05f);
            if (!played) PlayRandom(dropClips);
            return true;
        }

        public void PutBack(IFoodSource source)
        {
            if (!IsLoaded) return;
            source.Refill(1);
            PortionPool.Release(loadedVisual);
            loadedVisual = null;
            loadedDish = null;
            Pulse(0.2f, 0.05f);
            PlayRandom(dropClips);
        }

        bool PlayDishSound(AudioClip clip)
        {
            if (sfx == null || clip == null) return false;
            sfx.pitch = Random.Range(0.94f, 1.06f);
            sfx.PlayOneShot(clip);
            return true;
        }

        void PlayRandom(AudioClip[] clips)
        {
            if (sfx == null || clips == null || clips.Length == 0) return;
            sfx.pitch = Random.Range(0.95f, 1.05f);
            sfx.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        }

        void Update()
        {
            if (!IsLoaded || plateBelow == null) return;
            if (Vector3.Angle(transform.up, Vector3.up) > dropAngle) Drop(plateBelow);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!IsHeld) return;
            var source = other.GetComponentInParent<IFoodSource>();
            if (source != null)
            {
                if (IsLoaded) { if (source.Dish == loadedDish) PutBack(source); }
                else Scoop(source);
                return;
            }
            var plate = other.GetComponentInParent<IPlate>();
            if (plate != null) plateBelow = plate;
        }

        void OnTriggerExit(Collider other)
        {
            if (plateBelow != null && other.GetComponentInParent<IPlate>() == plateBelow) plateBelow = null;
        }
    }
}
