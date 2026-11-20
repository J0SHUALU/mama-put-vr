using TMPro;
using UnityEngine;

namespace MamaPut
{
    // Aunty Funmi's voice note. Each step waits for the player to actually do the thing.
    public class Tutorial : MonoBehaviour
    {
        [System.Serializable]
        public class Step
        {
            [TextArea] public string text;
            public string hint;
            public Transform pointAt;
            public AudioClip voice;
        }

        [SerializeField] TMP_Text body;
        [SerializeField] TMP_Text hintLabel;
        [SerializeField] TMP_Text counter;
        [SerializeField] Transform pointerRing;
        [SerializeField] AudioSource voiceSource;
        [SerializeField] Step[] steps;

        int index = -1;
        bool scooped;

        void Start()
        {
            if (SaveSystem.TutorialDone)
            {
                Finish();
                return;
            }
            GameEvents.OrderServed += OnServed;
            GameEvents.PaymentDone += OnPaid;
            Next();
        }

        void OnDestroy()
        {
            GameEvents.OrderServed -= OnServed;
            GameEvents.PaymentDone -= OnPaid;
        }

        void Update()
        {
            var step = index >= 0 && index < steps.Length ? steps[index] : null;
            if (pointerRing != null && step != null && step.pointAt != null)
            {
                float bob = Mathf.Sin(Time.time * 4f) * 0.02f;
                pointerRing.position = step.pointAt.position + Vector3.up * (0.18f + bob);
                float pulse = 1f + Mathf.Sin(Time.time * 6f) * 0.08f;
                pointerRing.localScale = Vector3.one * pulse;
            }

            // Scooping doesn't raise an event, so check the utensils until it happens.
            if (scooped || index != 0) return;
            foreach (var u in FindObjectsByType<Utensil>(FindObjectsSortMode.None))
                if (u.IsLoaded) { scooped = true; Next(); break; }
        }

        void OnServed(Customer c, bool correct) { if (correct && index == 1) Next(); }
        void OnPaid(Customer c, int amount, bool exact) { if (index == 2) Next(); }

        void Next()
        {
            index++;
            if (index >= steps.Length)
            {
                Finish();
                return;
            }
            var step = steps[index];
            body.text = step.text;
            hintLabel.text = step.hint;
            counter.text = $"{index + 1} / {steps.Length}";
            if (pointerRing != null) pointerRing.gameObject.SetActive(step.pointAt != null);
            if (voiceSource != null && step.voice != null)
            {
                voiceSource.clip = step.voice;
                voiceSource.Play();
            }
            if (index == steps.Length - 1) Invoke(nameof(Finish), 6f);
        }

        public void Skip() => Finish();

        void Finish()
        {
            SaveSystem.TutorialDone = true;
            if (pointerRing != null) pointerRing.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
