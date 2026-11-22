using TMPro;
using UnityEngine;

namespace MamaPut
{
    // Settings shared by the main menu and the pause menu. Opens where the menu that called it was.
    public class SettingsPanel : MonoBehaviour
    {
        [SerializeField] ComfortSettings comfort;
        [SerializeField] TMP_Text snapLabel;
        [SerializeField] TMP_Text seatedLabel;
        [SerializeField] TMP_Text volumeLabel;
        [SerializeField] AudioSource clickSound;

        const string VolumeKey = "mamaput.volume";
        const string SnapKey = "mamaput.snap";
        GameObject returnTo;

        void Awake()
        {
            AudioListener.volume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        }

        void Start()
        {
            if (comfort != null) comfort.SetSnapTurn(PlayerPrefs.GetInt(SnapKey, 0) == 1);
            Refresh();
        }

        public void Open(GameObject from)
        {
            returnTo = from;
            if (from != null)
            {
                transform.SetPositionAndRotation(from.transform.position, from.transform.rotation);
                from.SetActive(false);
            }
            gameObject.SetActive(true);
            Refresh();
        }

        public void Back()
        {
            Click();
            gameObject.SetActive(false);
            if (returnTo != null) returnTo.SetActive(true);
        }

        public void ToggleSnap()
        {
            Click();
            bool on = PlayerPrefs.GetInt(SnapKey, 0) != 1;
            PlayerPrefs.SetInt(SnapKey, on ? 1 : 0);
            if (comfort != null) comfort.SetSnapTurn(on);
            Refresh();
        }

        public void ToggleSeated()
        {
            Click();
            if (comfort != null) comfort.ToggleSeated();
            Refresh();
        }

        public void VolumeUp() => SetVolume(AudioListener.volume + 0.1f);
        public void VolumeDown() => SetVolume(AudioListener.volume - 0.1f);

        void SetVolume(float v)
        {
            AudioListener.volume = Mathf.Clamp01(Mathf.Round(v * 10f) / 10f);
            PlayerPrefs.SetFloat(VolumeKey, AudioListener.volume);
            Click();
            Refresh();
        }

        void Refresh()
        {
            if (snapLabel != null) snapLabel.text = PlayerPrefs.GetInt(SnapKey, 0) == 1 ? "On" : "Off";
            if (seatedLabel != null) seatedLabel.text = comfort != null && comfort.Seated ? "On" : "Off";
            if (volumeLabel != null) volumeLabel.text = Mathf.RoundToInt(AudioListener.volume * 100f) + "%";
        }

        void Click()
        {
            if (clickSound != null) clickSound.Play();
        }
    }
}
