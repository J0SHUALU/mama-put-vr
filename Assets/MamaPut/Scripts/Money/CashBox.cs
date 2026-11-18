using UnityEngine;

namespace MamaPut
{
    public class CashBox : MonoBehaviour, IPaymentHandler
    {
        [SerializeField] AudioSource dropSound;

        Customer payer;
        int received;

        public int ChangeOwed => payer == null ? 0 : Mathf.Max(0, received - payer.CurrentOrder.Total);

        public void ExpectPayment(Customer customer)
        {
            payer = customer;
            received = 0;
        }

        public void ReceivePayment(ICurrency note)
        {
            if (payer == null) return;
            received += note.Value;
            if (dropSound != null) dropSound.Play();
            if (received >= payer.CurrentOrder.Total)
            {
                var customer = payer;
                payer = null;
                customer.OnPaid(received, Mathf.Max(0, received - customer.CurrentOrder.Total));
            }
        }

        // Only count notes the player let go of inside the box, not ones they're waving around.
        void OnTriggerStay(Collider other)
        {
            var note = other.GetComponentInParent<NairaNote>();
            if (note == null || note.IsHeld || !note.gameObject.activeInHierarchy) return;
            ReceivePayment(note);
            note.Recycle();
        }
    }
}
