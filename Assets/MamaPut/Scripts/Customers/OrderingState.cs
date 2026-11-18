namespace MamaPut
{
    public class OrderingState : CustomerStateBase
    {
        public override void Enter(Customer c)
        {
            c.FaceStall(c.Queue.StallPoint);
            c.Bubble.ShowOrder(c.CurrentOrder);
            GameEvents.RaiseCustomerArrived(c);
        }

        public override void Tick(Customer c)
        {
            c.DrainPatience(1f);
            c.Bubble.SetPatience(c.PatienceLeft01);
            if (c.Patience <= 0f) c.WalkOut();
        }
    }
}
