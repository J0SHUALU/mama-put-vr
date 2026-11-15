using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MamaPut
{
    public class Order
    {
        public readonly Dictionary<DishData, int> Items = new Dictionary<DishData, int>();

        public int Total => Items.Sum(i => i.Key.pricePerPortion * i.Value);

        public void Add(DishData dish, int portions = 1)
        {
            Items.TryGetValue(dish, out var count);
            Items[dish] = count + portions;
        }

        // Exact match only. Extra food is still wrong, it costs Aunty money.
        public bool IsMatchedBy(PlateContents plate)
        {
            if (plate.TotalPortions != Items.Values.Sum()) return false;
            return Items.All(i => plate.CountOf(i.Key) == i.Value);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var item in Items)
            {
                if (sb.Length > 0) sb.Append(", ");
                sb.Append(item.Value > 1 ? $"{item.Value} {item.Key.displayName}" : item.Key.displayName);
            }
            return sb.ToString();
        }
    }
}
