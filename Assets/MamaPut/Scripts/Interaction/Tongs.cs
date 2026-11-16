namespace MamaPut
{
    public class Tongs : Utensil
    {
        public override bool CanServe(DishData dish) => dish.servedWithTongs;
    }
}
