namespace MamaPut
{
    public interface IOrderGenerator
    {
        Order CreateOrder(DayConfig day);
    }
}
