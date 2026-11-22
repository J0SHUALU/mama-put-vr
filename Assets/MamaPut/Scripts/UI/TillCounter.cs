using TMPro;
using UnityEngine;

namespace MamaPut
{
    // Floating till readout over the counter: today's takings, plus the change still owed.
    public class TillCounter : MonoBehaviour
    {
        [SerializeField] TMP_Text tillLabel;
        [SerializeField] TMP_Text changeLabel;
        [SerializeField] CashBox cashBox;

        int takings;
        bool newDay;
        int shownChange = -1;

        void OnEnable()
        {
            GameEvents.PaymentDone += OnPaid;
            GameEvents.DayEnded += OnDayEnded;
            Refresh();
        }

        void OnDisable()
        {
            GameEvents.PaymentDone -= OnPaid;
            GameEvents.DayEnded -= OnDayEnded;
        }

        void OnPaid(Customer c, int amount, bool exact)
        {
            if (newDay) { takings = 0; newDay = false; }
            takings += amount;
            Refresh();
        }

        void OnDayEnded(DaySummary s) => newDay = true;

        void Update()
        {
            int owed = 0;
            foreach (var c in FindObjectsByType<Customer>())
                if (c.isActiveAndEnabled && c.ChangeOwed > c.ChangeReceived && !c.WalkedOut)
                    owed = Mathf.Max(owed, c.ChangeOwed - c.ChangeReceived);
            if (owed == shownChange) return;
            shownChange = owed;
            if (changeLabel != null)
            {
                changeLabel.gameObject.SetActive(owed > 0);
                changeLabel.text = "Change owed " + Naira.Format(owed);
            }
        }

        void Refresh()
        {
            if (tillLabel != null) tillLabel.text = "Till " + Naira.Format(takings);
        }
    }
}
