namespace MamaPut
{
    public class PayingState : CustomerStateBase
    {
        public override void Enter(Customer c)
        {
            c.Bubble.ShowMessage("That's " + Naira.Format(c.CurrentOrder.Total));
            c.HandOverNotes();
        }

        public override void Tick(Customer c)
        {
            // Waiting for change is annoying but they already have their food.
            c.DrainPatience(0.5f);
            c.Bubble.SetPatience(c.PatienceLeft01);
            if (c.Patience <= 0f) c.WalkOut();
        }
    }
}
