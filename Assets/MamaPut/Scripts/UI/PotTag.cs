using TMPro;
using UnityEngine;

namespace MamaPut
{
    // The price tag clipped to each pot, plus the "don finish" warning when it runs out.
    public class PotTag : MonoBehaviour
    {
        [SerializeField] FoodPot pot;
        [SerializeField] TMP_Text nameLabel;
        [SerializeField] TMP_Text infoLabel;
        [SerializeField] GameObject warning;
        [SerializeField] TMP_Text warningLabel;

        int shown = -1;

        void Start()
        {
            nameLabel.text = pot.Dish.displayName;
            if (warningLabel != null) warningLabel.text = pot.Dish.displayName + " don finish";
        }

        void Update()
        {
            if (pot.PortionsLeft == shown) return;
            shown = pot.PortionsLeft;
            infoLabel.text = $"{Naira.Format(pot.Dish.pricePerPortion)} · {shown} left";
            if (warning != null) warning.SetActive(shown == 0);
        }
    }
}
