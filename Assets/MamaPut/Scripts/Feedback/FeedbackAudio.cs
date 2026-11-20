using UnityEngine;

namespace MamaPut
{
    public class FeedbackAudio : MonoBehaviour
    {
        [SerializeField] AudioSource source;
        [SerializeField] AudioClip correctPlate;
        [SerializeField] AudioClip wrongPlate;
        [SerializeField] AudioClip saleDone;
        [SerializeField] AudioClip walkout;
        [SerializeField] AudioClip potEmpty;
        [SerializeField] AudioClip dayEnd;

        void OnEnable()
        {
            GameEvents.OrderServed += OnServed;
            GameEvents.PaymentDone += OnPaid;
            GameEvents.CustomerWalkedOut += OnWalkout;
            GameEvents.PotEmpty += OnPotEmpty;
            GameEvents.DayEnded += OnDayEnded;
        }

        void OnDisable()
        {
            GameEvents.OrderServed -= OnServed;
            GameEvents.PaymentDone -= OnPaid;
            GameEvents.CustomerWalkedOut -= OnWalkout;
            GameEvents.PotEmpty -= OnPotEmpty;
            GameEvents.DayEnded -= OnDayEnded;
        }

        void OnServed(Customer c, bool correct) => Play(correct ? correctPlate : wrongPlate);
        void OnPaid(Customer c, int amount, bool exact) => Play(saleDone);
        void OnWalkout(Customer c) => Play(walkout);
        void OnPotEmpty(IFoodSource pot) => Play(potEmpty);
        void OnDayEnded(DaySummary s) => Play(dayEnd);

        void Play(AudioClip clip)
        {
            if (clip != null) source.PlayOneShot(clip);
        }
    }
}
