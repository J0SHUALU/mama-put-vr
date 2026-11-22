using UnityEngine;
using UnityEngine.InputSystem;

namespace MamaPut
{
    // A or X hands things across the counter, so you don't have to reach over it:
    // holding a plate serves it, holding change gives it, empty hand takes the customer's money.
    public class HandOverButton : MonoBehaviour
    {
        [SerializeField] QueueManager queue;
        [SerializeField] CashBox cashBox;

        InputAction press;

        void Awake()
        {
            press = new InputAction("HandOver", InputActionType.Button);
            press.AddBinding("<XRController>{RightHand}/primaryButton");
            press.AddBinding("<XRController>{LeftHand}/primaryButton");
            press.AddBinding("<Keyboard>/space");
            press.performed += _ => HandOver();
        }

        void OnEnable() => press.Enable();
        void OnDisable() => press.Disable();
        void OnDestroy() => press.Dispose();

        void HandOver()
        {
            if (PauseMenu.Paused || queue == null) return;
            var customer = queue.Front;
            if (customer == null) return;

            foreach (var plate in FindObjectsByType<Plate>(FindObjectsSortMode.None))
                if (plate.IsHeld && customer.IsOrdering)
                {
                    customer.TryServe(plate);
                    return;
                }

            foreach (var note in FindObjectsByType<NairaNote>(FindObjectsSortMode.None))
            {
                if (!note.IsHeld) continue;
                note.LetGo();
                if (customer.IsOwnNote(note)) PayIn(note);
                else customer.ReceiveChange(note);
                return;
            }

            // Nothing in hand: take whatever the customer is holding out.
            foreach (var note in customer.NotesOut)
                if (note != null && note.gameObject.activeInHierarchy) PayIn(note);
        }

        void PayIn(NairaNote note)
        {
            if (cashBox == null) return;
            cashBox.ReceivePayment(note);
            note.Recycle();
        }
    }
}
