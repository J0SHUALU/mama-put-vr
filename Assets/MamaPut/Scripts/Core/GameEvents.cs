using System;

namespace MamaPut
{
    public static class GameEvents
    {
        public static event Action<Customer> CustomerArrived;
        public static event Action<Customer, bool> OrderServed;
        public static event Action<Customer, int, bool> PaymentDone;
        public static event Action<Customer> CustomerWalkedOut;
        public static event Action<IFoodSource> PotEmpty;
        public static event Action<DaySummary> DayEnded;

        public static void RaiseCustomerArrived(Customer c) => CustomerArrived?.Invoke(c);
        public static void RaiseOrderServed(Customer c, bool correct) => OrderServed?.Invoke(c, correct);
        public static void RaisePaymentDone(Customer c, int amount, bool exact) => PaymentDone?.Invoke(c, amount, exact);
        public static void RaiseCustomerWalkedOut(Customer c) => CustomerWalkedOut?.Invoke(c);
        public static void RaisePotEmpty(IFoodSource pot) => PotEmpty?.Invoke(pot);
        public static void RaiseDayEnded(DaySummary s) => DayEnded?.Invoke(s);

        // Static events survive domain reload being off in the editor, so clear them on play.
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset()
        {
            CustomerArrived = null;
            OrderServed = null;
            PaymentDone = null;
            CustomerWalkedOut = null;
            PotEmpty = null;
            DayEnded = null;
        }
    }
}
