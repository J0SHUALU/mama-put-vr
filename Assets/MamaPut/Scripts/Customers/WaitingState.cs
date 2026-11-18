namespace MamaPut
{
    public class WaitingState : CustomerStateBase
    {
        public override void Enter(Customer c) => c.Queue.Join(c);

        public override void Tick(Customer c)
        {
            // People in the line get impatient too, just slower.
            c.DrainPatience(0.25f);
            if (c.Patience <= 0f)
            {
                c.WalkOut();
                return;
            }
            if (c.Queue.IsAtCounter(c) && c.AtTarget) c.SetState(new OrderingState());
        }
    }
}
