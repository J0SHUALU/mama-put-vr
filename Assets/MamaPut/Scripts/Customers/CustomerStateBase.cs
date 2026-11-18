namespace MamaPut
{
    public abstract class CustomerStateBase : ICustomerState
    {
        public virtual void Enter(Customer customer) { }
        public virtual void Tick(Customer customer) { }
        public virtual void Exit(Customer customer) { }
    }
}
