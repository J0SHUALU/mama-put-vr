using UnityEngine;

namespace MamaPut
{
    public class ScoreService : MonoBehaviour, IScoreService
    {
        [Tooltip("Share of sales with exact change needed for the accuracy star.")]
        [Range(0f, 1f)] [SerializeField] float exactChangeRatioForStar = 0.8f;

        DaySummary summary;

        public void BeginDay(DayConfig day)
        {
            summary = new DaySummary { dayNumber = day.dayNumber, target = day.target };
        }

        public void AddSale(int amount, bool exactChange, float patienceLeft)
        {
            summary.sales += amount;
            summary.customersServed++;
            if (exactChange) summary.exactChangeCount++;
        }

        public void AddWalkout() => summary.walkouts++;

        public DaySummary GetSummary()
        {
            var s = summary;
            s.stars = 0;
            if (s.TargetReached) s.stars++;
            if (s.walkouts == 0 && s.customersServed > 0) s.stars++;
            if (s.customersServed > 0 && (float)s.exactChangeCount / s.customersServed >= exactChangeRatioForStar) s.stars++;
            return s;
        }
    }
}
