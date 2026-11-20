using UnityEngine;

namespace MamaPut
{
    // Lives on the back table. Tip it into the matching pot to top it up, then it goes home.
    public class RefillBowl : GrabbableItem
    {
        [SerializeField] DishData dish;
        [SerializeField] float cooldown = 8f;
        [SerializeField] AudioSource refillSound;

        Vector3 homePosition;
        Quaternion homeRotation;
        Rigidbody body;
        float readyAt;

        protected override void Awake()
        {
            base.Awake();
            body = GetComponent<Rigidbody>();
            homePosition = transform.position;
            homeRotation = transform.rotation;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!IsHeld || Time.time < readyAt) return;
            var pot = other.GetComponentInParent<FoodPot>();
            if (pot == null || pot.Dish != dish) return;
            pot.RefillToFull();
            Pulse(0.6f, 0.15f);
            if (refillSound != null) refillSound.Play();
            readyAt = Time.time + cooldown;
            ForceRelease();
            GoHome();
        }

        protected override void OnReleased() => Invoke(nameof(GoHome), 2f);

        void GoHome()
        {
            if (IsHeld) return;
            if (body != null)
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }
            transform.SetPositionAndRotation(homePosition, homeRotation);
        }
    }
}
