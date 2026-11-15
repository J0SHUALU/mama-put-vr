namespace MamaPut
{
    public struct DaySummary
    {
        public int dayNumber;
        public int sales;
        public int target;
        public int customersServed;
        public int walkouts;
        public int exactChangeCount;
        public int stars;

        public bool TargetReached => sales >= target;
    }
}
