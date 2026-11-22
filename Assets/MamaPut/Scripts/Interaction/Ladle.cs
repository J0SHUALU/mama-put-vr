namespace MamaPut
{
    public class Ladle : Utensil
    {
        // Either utensil picks up any dish, so nobody gets stuck on the wrong tool.
        public override bool CanServe(DishData dish) => true;
    }
}
