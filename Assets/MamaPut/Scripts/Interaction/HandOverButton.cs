using UnityEngine;
using UnityEngine.InputSystem;

namespace MamaPut
{
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

            foreach (var plate in FindObjectsByType<Plate>())
                if (plate.IsHeld && customer.IsOrdering)
                {
                    customer.TryServe(plate);
                    return;
                }

            foreach (var note in FindObjectsByType<NairaNote>())
            {
                if (!note.IsHeld) continue;
                note.LetGo();
                if (customer.IsOwnNote(note)) PayIn(note);
                else customer.ReceiveChange(note);
                return;
            }

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
