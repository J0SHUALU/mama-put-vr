using System.Collections.Generic;
using UnityEngine;

namespace MamaPut
{
    public class Customer : MonoBehaviour, IPoolable
    {
        [SerializeField] float walkSpeed = 1.2f;
        [SerializeField] Transform hand;
        [SerializeField] OrderBubble bubble;
        [SerializeField] Animator animator;

        static readonly int SpeedHash = Animator.StringToHash("Speed");
        static readonly int IdleVariantHash = Animator.StringToHash("IdleVariant");
        const int IdleVariants = 3;

        ICustomerState state;
        Vector3 target;
        readonly List<NairaNote> notesOut = new List<NairaNote>();
        Plate plateHeld;
        Transform carry;
        CustomerPlateHold hold;

        public Order CurrentOrder { get; private set; }
        public float Patience { get; private set; }
        public float MaxPatience { get; private set; }
        public float PatienceLeft01 => MaxPatience <= 0 ? 0 : Patience / MaxPatience;
        public bool AtTarget => (transform.position - target).sqrMagnitude < 0.0025f;
        public int ChangeOwed { get; private set; }
        public int ChangeReceived { get; private set; }
        public bool Served { get; private set; }
        public bool WalkedOut { get; private set; }
        public bool ExactChange { get; private set; }
        public bool IsWalking { get; private set; }

        public Transform Hand => hand;
        public OrderBubble Bubble => bubble;
        public QueueManager Queue { get; private set; }
        public IPaymentHandler Till { get; private set; }
        public Vector3 ExitPoint { get; private set; }
        public bool WantsChange { get; private set; }
        public System.Action<Customer> ReturnToPool;

        void Awake()
        {
            carry = new GameObject("Carry").transform;
            carry.SetParent(transform, false);
            carry.localPosition = new Vector3(0f, 1f, 0.28f);
            if (animator != null)
            {
                hold = animator.GetComponent<CustomerPlateHold>();
                if (hold == null) hold = animator.gameObject.AddComponent<CustomerPlateHold>();
                hold.Setup(transform, carry);
            }
        }

        public void Begin(Order order, float patience, bool payExact, QueueManager queue, IPaymentHandler till, Vector3 exit)
        {
            CurrentOrder = order;
            MaxPatience = Patience = patience;
            WantsChange = !payExact;
            Queue = queue;
            Till = till;
            ExitPoint = exit;
            if (animator != null)
            {
                // Different idle, start frame and pace for each visit so the line looks like people.
                animator.SetFloat(IdleVariantHash, Random.Range(0, IdleVariants));
                animator.speed = Random.Range(0.9f, 1.1f);
                animator.Play("Idle", 0, Random.value);
            }
            SetState(new WaitingState());
        }

        public void SetState(ICustomerState next)
        {
            state?.Exit(this);
            state = next;
            state.Enter(this);
        }

        public void WalkTo(Vector3 point) => target = point;

        public void DrainPatience(float rate) => Patience = Mathf.Max(0f, Patience - Time.deltaTime * rate);

        public void LosePatience(float amount) => Patience = Mathf.Max(0f, Patience - amount);

        void Update()
        {
            var to = target - transform.position;
            to.y = 0f;
            float step = walkSpeed * Time.deltaTime;
            if (to.magnitude > step)
            {
                transform.position += to.normalized * step;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(to), 10f * Time.deltaTime);
            }
            else transform.position = new Vector3(target.x, transform.position.y, target.z);

            IsWalking = to.magnitude > step;
            if (animator != null) animator.SetFloat(SpeedHash, IsWalking ? walkSpeed : 0f);
            state?.Tick(this);
        }

        public void FaceStall(Vector3 stallPoint)
        {
            var dir = stallPoint - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f) transform.rotation = Quaternion.LookRotation(dir);
        }

        // Called by CustomerHandZone when the player lets go of a plate near the customer.
        public void TryServe(Plate plate)
        {
            if (!(state is OrderingState) || plateHeld != null) return;
            if (CurrentOrder.IsMatchedBy(plate.Contents))
            {
                plateHeld = plate;
                plate.HandTo(carry);
                if (hold != null) hold.Hold(plate.transform);
                Served = true;
                GameEvents.RaiseOrderServed(this, true);
                SetState(new PayingState());
            }
            else
            {
                LosePatience(5f);
                // Say what was wrong, so the player can fix the plate instead of guessing.
                bubble.Complain($"I want {CurrentOrder}. This one na {plate.Contents}!");
                Debug.Log($"Wrong plate: ordered {CurrentOrder}, got {plate.Contents}");
                GameEvents.RaiseOrderServed(this, false);
            }
        }

        public void HandOverNotes()
        {
            var notes = CustomerWallet.Instance.ChooseNotes(CurrentOrder.Total, !WantsChange);
            for (int i = 0; i < notes.Count; i++) notesOut.Add(CustomerWallet.Instance.Spawn(notes[i], hand, i + 1));
            Till.ExpectPayment(this);
        }

        public bool IsOwnNote(NairaNote note) => notesOut.Contains(note);
        public NairaNote[] NotesOut => notesOut.ToArray();
        public bool IsOrdering => state is OrderingState;

        public int PaidAmount { get; private set; }

        public void OnPaid(int received, int changeOwed)
        {
            PaidAmount = received;
            ChangeOwed = changeOwed;
            ChangeReceived = 0;
            notesOut.Clear();
            if (changeOwed == 0) Finish(true);
            else bubble.ShowChange(PaidAmount, changeOwed);
        }

        // Called by CustomerHandZone when the player drops change notes near the customer.
        public void ReceiveChange(NairaNote note)
        {
            if (!(state is PayingState) || ChangeOwed == 0) return;
            ChangeReceived += note.Value;
            note.Recycle();
            if (ChangeReceived >= ChangeOwed) Finish(ChangeReceived == ChangeOwed);
            else bubble.ShowChange(PaidAmount, ChangeOwed - ChangeReceived);
        }

        void Finish(bool exact)
        {
            ExactChange = exact;
            if (!exact) bubble.Complain("You give me extra o. Thank you!");
            GameEvents.RaisePaymentDone(this, CurrentOrder.Total, exact);
            SetState(new LeavingState());
        }

        public void WalkOut()
        {
            WalkedOut = true;
            bubble.Complain("I no fit wait again!");
            foreach (var n in notesOut) if (n != null && n.gameObject.activeInHierarchy) n.Recycle();
            notesOut.Clear();
            GameEvents.RaiseCustomerWalkedOut(this);
            SetState(new LeavingState());
        }

        public void Despawn()
        {
            if (ReturnToPool != null) ReturnToPool(this);
            else Destroy(gameObject);
        }

        public void OnSpawn()
        {
            Served = WalkedOut = ExactChange = false;
            ChangeOwed = ChangeReceived = 0;
            target = transform.position;
        }

        public void OnDespawn()
        {
            if (plateHeld != null) plateHeld.Recycle();
            plateHeld = null;
            if (hold != null) hold.Hold(null);
            state = null;
            bubble.Hide();
        }
    }
}
