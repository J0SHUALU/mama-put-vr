using UnityEngine;

namespace MamaPut
{
    public static class SaveSystem
    {
        const string DayKey = "mamaput.unlockedDay";
        const string StarsKey = "mamaput.stars.";
        const string TutorialKey = "mamaput.tutorialDone";

        public static int UnlockedDay
        {
            get => PlayerPrefs.GetInt(DayKey, 1);
            set { PlayerPrefs.SetInt(DayKey, value); PlayerPrefs.Save(); }
        }

        public static bool TutorialDone
        {
            get => PlayerPrefs.GetInt(TutorialKey, 0) == 1;
            set { PlayerPrefs.SetInt(TutorialKey, value ? 1 : 0); PlayerPrefs.Save(); }
        }

        public static int BestStars(int day) => PlayerPrefs.GetInt(StarsKey + day, 0);

        public static void SaveStars(int day, int stars)
        {
            if (stars <= BestStars(day)) return;
            PlayerPrefs.SetInt(StarsKey + day, stars);
            PlayerPrefs.Save();
        }

        public static void Wipe()
        {
            PlayerPrefs.DeleteKey(DayKey);
            PlayerPrefs.DeleteKey(TutorialKey);
            for (int d = 1; d <= 10; d++) PlayerPrefs.DeleteKey(StarsKey + d);
        }
    }
}
