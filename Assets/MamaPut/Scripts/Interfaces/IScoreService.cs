namespace MamaPut
{
    public interface IScoreService
    {
        void BeginDay(DayConfig day);
        void AddSale(int amount, bool exactChange, float patienceLeft);
        void AddWalkout();
        DaySummary GetSummary();
    }
}
