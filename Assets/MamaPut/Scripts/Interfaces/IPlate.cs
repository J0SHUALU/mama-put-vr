namespace MamaPut
{
    public interface IPlate
    {
        PlateContents Contents { get; }
        bool AddPortion(DishData dish);
        void Clear();
    }
}
