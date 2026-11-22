using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace MamaPut
{
    // Menu button on the left controller (or Escape in the editor) pauses the game and shows this in front of you.
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] GameObject panel;
        [SerializeField] SettingsPanel settings;
        [SerializeField] float distance = 0.9f;
        [SerializeField] AudioSource clickSound;

        InputAction toggle;
        Transform view;
        public static bool Paused { get; private set; }

        void Awake()
        {
            toggle = new InputAction("Pause", InputActionType.Button);
            toggle.AddBinding("<XRController>{LeftHand}/menuButton");
            toggle.AddBinding("<Keyboard>/escape");
            toggle.performed += _ => Toggle();
            if (panel != null) panel.SetActive(false);
            Paused = false;
        }

        void OnEnable() => toggle.Enable();
        void OnDisable() => toggle.Disable();
        void OnDestroy() => toggle.Dispose();

        void Toggle()
        {
            if (!MainMenu.InGame) return;
            if (Paused) Resume();
            else Pause();
        }

        public void Pause()
        {
            Paused = true;
            Time.timeScale = 0f;
            Place();
            panel.SetActive(true);
            Click();
        }

        public void Resume()
        {
            Click();
            if (settings != null) settings.gameObject.SetActive(false);
            panel.SetActive(false);
            Time.timeScale = 1f;
            Paused = false;
        }

        public void OpenSettings()
        {
            Click();
            if (settings != null) settings.Open(panel);
        }

        public void BackToMainMenu()
        {
            Time.timeScale = 1f;
            Paused = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // Float the panel at eye height straight ahead of wherever the player is looking.
        void Place()
        {
            if (view == null)
            {
                var cam = Camera.main != null ? Camera.main : FindAnyObjectByType<Camera>();
                if (cam != null) view = cam.transform;
            }
            if (view == null) return;
            var flat = view.forward;
            flat.y = 0f;
            if (flat.sqrMagnitude < 0.001f) flat = Vector3.forward;
            flat.Normalize();
            panel.transform.SetPositionAndRotation(view.position + flat * distance, Quaternion.LookRotation(flat));
        }

        void Click()
        {
            if (clickSound != null) clickSound.Play();
        }
    }
}
