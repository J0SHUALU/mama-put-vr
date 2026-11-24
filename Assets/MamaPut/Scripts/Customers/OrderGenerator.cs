using System.Collections.Generic;
using UnityEngine;

namespace MamaPut
{
    public class OrderGenerator : MonoBehaviour, IOrderGenerator
    {
        [Tooltip("Max portions of a tongs dish (dodo) in one order.")]
        [SerializeField] int maxTongsPortions = 2;

        public Order CreateOrder(DayConfig day)
        {
            var order = new Order();
            var ladle = new List<DishData>();
            var tongs = new List<DishData>();
            foreach (var d in day.menu) (d.servedWithTongs ? tongs : ladle).Add(d);

            if (ladle.Count > 0) order.Add(ladle[Random.Range(0, ladle.Count)]);

            int extras = Random.Range(0, day.maxItemsPerOrder);
            for (int i = 0; i < extras; i++)
            {
                bool pickTongs = tongs.Count > 0 && (ladle.Count < 2 || Random.value < 0.5f);
                var pool = pickTongs ? tongs : ladle;
                if (pool.Count == 0) continue;
                order.Add(pool[Random.Range(0, pool.Count)], pickTongs ? Random.Range(1, maxTongsPortions + 1) : 1);
            }
            return order;
        }
    }
}
