using UnityEngine;

namespace MamaPut
{
    // Trigger in front of the customer. Plates and change get handed over by letting go inside it.
    public class CustomerHandZone : MonoBehaviour
    {
        [SerializeField] Customer customer;

        void OnTriggerStay(Collider other)
        {
            var plate = other.GetComponentInParent<Plate>();
            if (plate != null && !plate.IsHeld && plate.transform.parent != customer.Hand)
            {
                customer.TryServe(plate);
                return;
            }

            var note = other.GetComponentInParent<NairaNote>();
            if (note != null && !note.IsHeld && note.gameObject.activeInHierarchy && !customer.IsOwnNote(note))
                customer.ReceiveChange(note);
        }
    }
}
