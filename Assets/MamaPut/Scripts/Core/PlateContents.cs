using System.Collections.Generic;
using System.Linq;

namespace MamaPut
{
    public class PlateContents
    {
        readonly Dictionary<DishData, int> portions = new Dictionary<DishData, int>();

        public int TotalPortions => portions.Values.Sum();
        public IReadOnlyDictionary<DishData, int> Portions => portions;

        public void Add(DishData dish)
        {
            portions.TryGetValue(dish, out var count);
            portions[dish] = count + 1;
        }

        public int CountOf(DishData dish) => portions.TryGetValue(dish, out var c) ? c : 0;

        public int Price() => portions.Sum(p => p.Key.pricePerPortion * p.Value);

        public void Clear() => portions.Clear();
    }
}
