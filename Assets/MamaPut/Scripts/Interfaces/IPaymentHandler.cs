namespace MamaPut
{
    public interface IPaymentHandler
    {
        int ChangeOwed { get; }
        void ExpectPayment(Customer customer);
        void ReceivePayment(ICurrency note);
    }
}
