using System.Collections.Generic;
using UnityEngine;

namespace MamaPut
{
    [CreateAssetMenu(menuName = "Mama Put/Day", fileName = "Day")]
    public class DayConfig : ScriptableObject
    {
        public int dayNumber = 1;
        public int customerCount = 5;
        public float patienceSeconds = 60f;
        public float secondsBetweenCustomers = 6f;
        public int maxItemsPerOrder = 2;
        public int target = 2000;
        public bool exactMoneyOnly = true;
        public List<DishData> menu = new List<DishData>();
        [TextArea] public string briefing = "";
    }
}
