using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MamaPut
{
    public class SummaryBoard : MonoBehaviour
    {
        [SerializeField] DayManager dayManager;

        [Header("Number plate")]
        [SerializeField] TMP_Text plateTop;
        [SerializeField] TMP_Text plateDay;

        [Header("Before the day")]
        [SerializeField] GameObject readyGroup;
        [SerializeField] TMP_Text customersLabel;
        [SerializeField] TMP_Text targetLabel;
        [SerializeField] TMP_Text patienceLabel;
        [SerializeField] TMP_Text briefingLabel;

        [Header("After the day")]
        [SerializeField] GameObject resultGroup;
        [SerializeField] Image[] stars;
        [SerializeField] Image salesFill;
        [SerializeField] TMP_Text salesLabel;
        [SerializeField] TMP_Text servedLabel;
        [SerializeField] TMP_Text walkedLabel;
        [SerializeField] TMP_Text exactLabel;
        [SerializeField] TMP_Text quoteLabel;

        [Header("While serving")]
        [SerializeField] GameObject runningGroup;

        [Header("Buttons")]
        [SerializeField] GameObject startButton;
        [SerializeField] GameObject nextButton;
        [SerializeField] GameObject retryButton;

        [SerializeField] AudioSource clickSound;

        [SerializeField] Color starOn = new Color32(0xEF, 0x9F, 0x27, 0xFF);
        [SerializeField] Color starOff = new Color32(0x5F, 0x5E, 0x5A, 0xFF);

        static readonly string[] Quotes =
        {
            "\"Ah. Tomorrow is another day.\"",
            "\"Not bad. Count your change well.\"",
            "\"Well done o. Customers are talking.\"",
            "\"You try! I'm sending money for a bigger pot.\""
        };

        void OnEnable() => GameEvents.DayEnded += ShowResults;
        void OnDisable() => GameEvents.DayEnded -= ShowResults;

        void Start() => ShowReady();

        public void ShowReady()
        {
            var day = dayManager.CurrentDay;
            SetPlate("LAGOS", day.dayNumber);
            customersLabel.text = day.customerCount.ToString();
            targetLabel.text = Naira.Format(day.target);
            patienceLabel.text = Mathf.RoundToInt(day.patienceSeconds) + "s";
            briefingLabel.text = day.briefing;
            Show(readyGroup);
            SetButtons(true, false, false);
        }

        void ShowResults(DaySummary s)
        {
            SetPlate("DONE", s.dayNumber);
            for (int i = 0; i < stars.Length; i++) stars[i].color = i < s.stars ? starOn : starOff;
            salesFill.fillAmount = s.target > 0 ? Mathf.Clamp01((float)s.sales / s.target) : 0f;
            salesLabel.text = $"{Naira.Format(s.sales)} of {Naira.Format(s.target)}";
            servedLabel.text = $"{s.customersServed} served";
            walkedLabel.text = $"{s.walkouts} left";
            exactLabel.text = $"{s.exactChangeCount} exact";
            quoteLabel.text = Quotes[Mathf.Clamp(s.stars, 0, Quotes.Length - 1)];
            Show(resultGroup);
            SetButtons(false, dayManager.HasNextDay, true);
        }

        public void PressStart() => Begin(dayManager.StartDay);
        public void PressNext() => Begin(dayManager.NextDay);
        public void PressRetry() => Begin(dayManager.RetryDay);

        void Begin(System.Action action)
        {
            if (clickSound != null) clickSound.Play();
            action();
            SetPlate("OPEN", dayManager.CurrentDay.dayNumber);
            Show(runningGroup);
            SetButtons(false, false, false);
        }

        void SetPlate(string top, int day)
        {
            plateTop.text = top;
            plateDay.text = $"DAY · {day:00}";
        }

        void Show(GameObject group)
        {
            readyGroup.SetActive(group == readyGroup);
            resultGroup.SetActive(group == resultGroup);
            runningGroup.SetActive(group == runningGroup);
        }

        void SetButtons(bool start, bool next, bool retry)
        {
            startButton.SetActive(start);
            nextButton.SetActive(next);
            retryButton.SetActive(retry);
        }
    }
}
