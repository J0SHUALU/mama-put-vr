namespace MamaPut
{
    public class LeavingState : CustomerStateBase
    {
        public override void Enter(Customer c)
        {
            c.Queue.Leave(c);
            c.WalkTo(c.ExitPoint);
        }

        public override void Tick(Customer c)
        {
            if (c.AtTarget) c.Despawn();
        }
    }
}
