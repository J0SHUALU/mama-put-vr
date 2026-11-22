using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MamaPut
{
    // Pops up in front of the player when a day ends: how it went, what to do better, and where to go next.
    public class ResultsPanel : MonoBehaviour
    {
        [SerializeField] GameObject panel;
        [SerializeField] TMP_Text title;
        [SerializeField] TMP_Text stats;
        [SerializeField] TMP_Text tips;
        [SerializeField] GameObject continueButton;
        [SerializeField] SummaryBoard board;
        [SerializeField] DayManager dayManager;
        [SerializeField] AudioSource clickSound;
        [SerializeField] float distance = 0.9f;

        void Awake()
        {
            if (panel != null) panel.SetActive(false);
            GameEvents.DayEnded += Show;
        }

        void OnDestroy() => GameEvents.DayEnded -= Show;

        void Show(DaySummary s)
        {
            title.text = s.TargetReached ? $"Day {s.dayNumber} complete" : $"Day {s.dayNumber}: target missed";

            stats.text =
                $"Sales  {Naira.Format(s.sales)} of {Naira.Format(s.target)}\n" +
                $"Served  {s.customersServed}\n" +
                $"Walked off  {s.walkouts}\n" +
                $"Exact change  {s.exactChangeCount} of {s.customersServed}\n" +
                $"Stars  {new string('*', s.stars)}{new string('-', Mathf.Max(0, 3 - s.stars))}";

            var sb = new StringBuilder();
            if (s.walkouts > 0) sb.AppendLine($"Serve faster: {s.walkouts} customer{(s.walkouts > 1 ? "s" : "")} gave up waiting.");
            int wrongChange = s.customersServed - s.exactChangeCount;
            if (wrongChange > 0) sb.AppendLine($"Count your change: {wrongChange} got the wrong amount back.");
            if (!s.TargetReached) sb.AppendLine($"You were {Naira.Format(s.target - s.sales)} short of the target.");
            if (sb.Length == 0) sb.Append("Clean day. Nobody waited too long and every change was right.");
            tips.text = sb.ToString().TrimEnd();

            bool canContinue = s.TargetReached && dayManager != null && dayManager.HasNextDay;
            if (continueButton != null) continueButton.SetActive(canContinue);

            Place();
            panel.SetActive(true);
        }

        public void Retry()
        {
            Click();
            panel.SetActive(false);
            if (board != null) board.PressRetry();
        }

        public void Continue()
        {
            Click();
            panel.SetActive(false);
            if (board != null) board.PressNext();
        }

        public void MainMenu()
        {
            Click();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        void Place()
        {
            var cam = Camera.main != null ? Camera.main : FindAnyObjectByType<Camera>();
            if (cam == null) return;
            var flat = cam.transform.forward;
            flat.y = 0f;
            if (flat.sqrMagnitude < 0.001f) flat = Vector3.forward;
            flat.Normalize();
            panel.transform.SetPositionAndRotation(cam.transform.position + flat * distance, Quaternion.LookRotation(flat));
        }

        void Click()
        {
            if (clickSound != null) clickSound.Play();
        }
    }
}
