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
            [Tooltip("Short tag floating over the thing, e.g. \"Grab this\".")]
            public string pointLabel;
            public AudioClip voice;
        }

        [SerializeField] TMP_Text body;
        [SerializeField] TMP_Text hintLabel;
        [SerializeField] TMP_Text counter;
        [SerializeField] Transform pointerRing;
        [SerializeField] TMP_Text pointerLabel;
        [SerializeField] AudioSource voiceSource;
        [SerializeField] Step[] steps;

        int index = -1;
        bool scooped;
        Vector3 ringScale = Vector3.one;
        Transform view;

        void Start()
        {
            // The ring lives on a world-space canvas at a tiny scale; pulse around that, not around 1.
            if (pointerRing != null) ringScale = pointerRing.localScale;
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
                pointerRing.localScale = ringScale * pulse;

                if (pointerLabel != null)
                {
                    if (view == null) view = FindView();
                    var tag = pointerLabel.canvas.transform;
                    tag.position = step.pointAt.position + Vector3.up * (0.27f + bob);
                    if (view != null)
                    {
                        var away = tag.position - view.position;
                        away.y = 0f;
                        if (away.sqrMagnitude > 0.0001f) tag.rotation = Quaternion.LookRotation(away);
                    }
                }
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
            if (pointerLabel != null)
            {
                pointerLabel.text = string.IsNullOrEmpty(step.pointLabel) ? "Grab this" : step.pointLabel;
                pointerLabel.canvas.gameObject.SetActive(step.pointAt != null);
            }
            if (voiceSource != null && step.voice != null)
            {
                voiceSource.clip = step.voice;
                voiceSource.Play();
            }
            if (index == steps.Length - 1) Invoke(nameof(Finish), 6f);
        }

        public void Skip() => Finish();

        // The rig's camera isn't tagged MainCamera, so fall back to any active camera.
        static Transform FindView()
        {
            if (Camera.main != null) return Camera.main.transform;
            var cam = FindAnyObjectByType<Camera>();
            return cam != null ? cam.transform : null;
        }

        void Finish()
        {
            SaveSystem.TutorialDone = true;
            if (pointerRing != null) pointerRing.gameObject.SetActive(false);
            if (pointerLabel != null) pointerLabel.canvas.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
