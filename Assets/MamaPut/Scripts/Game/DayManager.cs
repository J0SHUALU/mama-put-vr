using System.Collections;
using UnityEngine;

namespace MamaPut
{
    public class DayManager : MonoBehaviour
    {
        [SerializeField] DayConfig[] days;
        [SerializeField] Customer[] customerPrefabs;
        [SerializeField] Transform spawnPoint;
        [SerializeField] Transform exitPoint;
        [SerializeField] QueueManager queue;
        [SerializeField] CashBox cashBox;

        // Unity can't serialize interfaces, so these are assigned as components and cast.
        [SerializeField] MonoBehaviour orderGeneratorComponent;
        [SerializeField] MonoBehaviour scoreServiceComponent;

        IOrderGenerator orders;
        IScoreService score;
        ObjectPool<Customer>[] pools;
        int dayIndex;
        int spawned;
        int finished;
        bool running;

        public DayConfig CurrentDay => days[dayIndex];
        public bool IsRunning => running;
        public bool HasNextDay => dayIndex + 1 < days.Length;

        void Awake()
        {
            orders = orderGeneratorComponent as IOrderGenerator;
            score = scoreServiceComponent as IScoreService;
            pools = new ObjectPool<Customer>[customerPrefabs.Length];
            for (int i = 0; i < customerPrefabs.Length; i++)
                pools[i] = new ObjectPool<Customer>(customerPrefabs[i], transform, 3);
            dayIndex = Mathf.Clamp(SaveSystem.UnlockedDay - 1, 0, days.Length - 1);
        }

        void OnEnable()
        {
            GameEvents.PaymentDone += HandlePayment;
            GameEvents.CustomerWalkedOut += HandleWalkout;
        }

        void OnDisable()
        {
            GameEvents.PaymentDone -= HandlePayment;
            GameEvents.CustomerWalkedOut -= HandleWalkout;
        }

        public void StartDay()
        {
            if (running) return;
            StartCoroutine(RunDay());
        }

        public void NextDay()
        {
            if (HasNextDay) dayIndex++;
            StartDay();
        }

        public void RetryDay() => StartDay();

        public void GoToDay(int dayNumber)
        {
            if (running) return;
            dayIndex = Mathf.Clamp(dayNumber - 1, 0, Mathf.Min(days.Length, SaveSystem.UnlockedDay) - 1);
        }

        IEnumerator RunDay()
        {
            running = true;
            spawned = finished = 0;
            score.BeginDay(CurrentDay);

            var wait = new WaitForSeconds(CurrentDay.secondsBetweenCustomers);
            while (spawned < CurrentDay.customerCount)
            {
                // Don't let the line get silly long. Four waiting is enough pressure.
                if (queue.Count < 4) SpawnCustomer();
                yield return wait;
            }

            while (finished < spawned) yield return null;
            EndDay();
        }

        void SpawnCustomer()
        {
            var pool = pools[Random.Range(0, pools.Length)];
            var c = pool.Get(spawnPoint.position, spawnPoint.rotation);
            c.ReturnToPool = pool.Release;
            c.Begin(orders.CreateOrder(CurrentDay), CurrentDay.patienceSeconds, CurrentDay.exactMoneyOnly, queue, cashBox, exitPoint.position);
            spawned++;
        }

        void HandlePayment(Customer c, int amount, bool exact)
        {
            score.AddSale(amount, exact, c.PatienceLeft01);
            finished++;
        }

        void HandleWalkout(Customer c)
        {
            score.AddWalkout();
            finished++;
        }

        void EndDay()
        {
            running = false;
            var summary = score.GetSummary();
            SaveSystem.SaveStars(summary.dayNumber, summary.stars);
            if (summary.TargetReached && SaveSystem.UnlockedDay <= summary.dayNumber)
                SaveSystem.UnlockedDay = Mathf.Min(summary.dayNumber + 1, days.Length);
            GameEvents.RaiseDayEnded(summary);
        }
    }
}
