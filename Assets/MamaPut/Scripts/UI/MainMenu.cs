using UnityEngine;

namespace MamaPut
{
    public class MainMenu : MonoBehaviour
    {
        [Tooltip("Shown after Play: the day board, comfort panel and so on.")]
        [SerializeField] GameObject[] showOnPlay;
        [Tooltip("Only shown after Play if the player hasn't finished it before.")]
        [SerializeField] GameObject tutorial;
        [SerializeField] SettingsPanel settings;
        [SerializeField] AudioSource clickSound;
        [Tooltip("Plays while the menu is up.")]
        [SerializeField] AudioSource menuMusic;
        [Tooltip("Street ambience that starts the moment you press Play.")]
        [SerializeField] AudioSource[] gameAmbience;

        public static bool InGame { get; private set; }

        void Awake()
        {
            InGame = false;
            foreach (var go in showOnPlay) if (go != null) go.SetActive(false);
            if (tutorial != null) tutorial.SetActive(false);
            foreach (var a in gameAmbience) if (a != null) a.Stop();
            if (menuMusic != null) menuMusic.Play();
        }

        public void Play()
        {
            Click();
            InGame = true;
            if (menuMusic != null) menuMusic.Stop();
            foreach (var a in gameAmbience) if (a != null) a.Play();
            foreach (var go in showOnPlay) if (go != null) go.SetActive(true);
            if (tutorial != null && !SaveSystem.TutorialDone) tutorial.SetActive(true);
            gameObject.SetActive(false);
        }

        public void OpenSettings()
        {
            Click();
            if (settings != null) settings.Open(gameObject);
        }

        public void Quit()
        {
            Click();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        void Click()
        {
            if (clickSound != null) clickSound.Play();
        }
    }
}
