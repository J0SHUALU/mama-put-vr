namespace MamaPut
{
    public interface IFoodSource
    {
        DishData Dish { get; }
        int PortionsLeft { get; }
        bool TakePortion();
        void Refill(int amount);
    }
}
