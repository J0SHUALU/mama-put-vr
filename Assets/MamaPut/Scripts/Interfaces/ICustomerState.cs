namespace MamaPut
{
    public interface ICustomerState
    {
        void Enter(Customer customer);
        void Tick(Customer customer);
        void Exit(Customer customer);
    }
}
