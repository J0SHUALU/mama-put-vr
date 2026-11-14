namespace MamaPut
{
    public interface IUtensil
    {
        bool IsLoaded { get; }
        bool CanServe(DishData dish);
        bool Scoop(IFoodSource source);
        bool Drop(IPlate target);
    }
}
