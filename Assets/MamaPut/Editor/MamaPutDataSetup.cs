using System.IO;
using UnityEditor;
using UnityEngine;

namespace MamaPut.EditorTools
{
    public static class MamaPutDataSetup
    {
        const string Folder = "Assets/MamaPut/Data";

        [MenuItem("Mama Put/Create Default Dishes and Days")]
        public static void CreateDefaults()
        {
            Directory.CreateDirectory(Folder + "/Dishes");
            Directory.CreateDirectory(Folder + "/Days");

            var rice = Dish("White Rice", 300, false, "R", Hex("F1EFE8"), Hex("2C2C2A"));
            var jollof = Dish("Jollof", 400, false, "J", Hex("D85A30"), Hex("FAECE7"));
            var stew = Dish("Stew", 100, false, "S", Hex("A32D2D"), Hex("FCEBEB"));
            var dodo = Dish("Dodo", 200, true, "D", Hex("EF9F27"), Hex("412402"));
            var meat = Dish("Meat", 300, true, "M", Hex("633806"), Hex("FAEEDA"));
            var beans = Dish("Beans", 300, false, "B", Hex("854F0B"), Hex("FAEEDA"));

            // Matches the progression table in the GDD.
            Day(1, 6, 2000, 70f, true, 1, "Rice and stew only. Everybody pays exact.", rice, stew);
            Day(2, 8, 3500, 60f, false, 2, "Jollof and dodo are out. People need change now.", rice, jollof, stew, dodo);
            Day(3, 10, 5000, 55f, false, 3, "Meat on the menu. Hagglers about.", rice, jollof, stew, dodo, meat);
            Day(4, 13, 7500, 50f, false, 3, "Rush hour. Keep an eye on the pots.", rice, jollof, stew, dodo, meat);
            Day(5, 16, 10000, 45f, false, 3, "Lunch rush and beans. Nobody wants to wait.", rice, jollof, stew, dodo, meat, beans);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Mama Put: dishes and days created in " + Folder + ". Assign a portion prefab to each dish.");
        }

        static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString("#" + hex, out var c);
            return c;
        }

        static DishData Dish(string name, int price, bool tongs, string label, Color chip, Color chipText)
        {
            string path = $"{Folder}/Dishes/{name}.asset";
            var dish = AssetDatabase.LoadAssetAtPath<DishData>(path);
            if (dish == null)
            {
                dish = ScriptableObject.CreateInstance<DishData>();
                AssetDatabase.CreateAsset(dish, path);
            }
            dish.displayName = name;
            dish.pricePerPortion = price;
            dish.servedWithTongs = tongs;
            dish.shortLabel = label;
            dish.chipColor = chip;
            dish.chipTextColor = chipText;
            EditorUtility.SetDirty(dish);
            return dish;
        }

        static void Day(int number, int customers, int target, float patience, bool exact, int maxItems, string briefing, params DishData[] menu)
        {
            string path = $"{Folder}/Days/Day {number}.asset";
            var day = AssetDatabase.LoadAssetAtPath<DayConfig>(path);
            if (day == null)
            {
                day = ScriptableObject.CreateInstance<DayConfig>();
                AssetDatabase.CreateAsset(day, path);
            }
            day.dayNumber = number;
            day.customerCount = customers;
            day.target = target;
            day.patienceSeconds = patience;
            day.exactMoneyOnly = exact;
            day.maxItemsPerOrder = maxItems;
            day.secondsBetweenCustomers = Mathf.Lerp(8f, 4f, (number - 1) / 4f);
            day.menu = new System.Collections.Generic.List<DishData>(menu);
            day.briefing = briefing;
            EditorUtility.SetDirty(day);
        }

        [MenuItem("Mama Put/Reset Save Data")]
        static void ResetSave()
        {
            SaveSystem.Wipe();
            Debug.Log("Mama Put: save data cleared.");
        }
    }
}
