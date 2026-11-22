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
        [Tooltip("Ladle dishes are scooped, tongs dishes are picked (dodo).")]
        public bool servedWithTongs;

        [Header("Sound")]
        [Tooltip("Played when this dish is scooped from its pot. Falls back to the utensil's own sounds.")]
        public AudioClip scoopSound;
        [Tooltip("Played when this dish lands on a plate.")]
        public AudioClip serveSound;

        [Header("Order chip")]
        public string shortLabel = "R";
        public Color chipColor = Color.white;
        public Color chipTextColor = Color.black;
    }
}
