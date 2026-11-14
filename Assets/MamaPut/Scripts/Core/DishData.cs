using UnityEngine;

namespace MamaPut
{
    [CreateAssetMenu(menuName = "Mama Put/Dish", fileName = "Dish")]
    public class DishData : ScriptableObject
    {
        public string displayName = "Rice";
        [Tooltip("Price in Naira for one portion.")]
        public int pricePerPortion = 300;
        public GameObject portionPrefab;
        [Tooltip("Ladle dishes are scooped, tongs dishes are picked (meat, dodo).")]
        public bool servedWithTongs;

        [Header("Order chip")]
        public string shortLabel = "R";
        public Color chipColor = Color.white;
        public Color chipTextColor = Color.black;
    }
}
