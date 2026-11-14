namespace MamaPut
{
    public interface IServable
    {
        PlateContents Contents { get; }
        int Price();
    }
}
