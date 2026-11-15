namespace MamaPut
{
    public class Ladle : Utensil
    {
        public override bool CanServe(DishData dish) => !dish.servedWithTongs;
    }
}
