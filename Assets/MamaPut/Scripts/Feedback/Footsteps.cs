using UnityEngine;

namespace MamaPut
{
    [RequireComponent(typeof(AudioSource))]
    public class Footsteps : MonoBehaviour
    {
        [SerializeField] Customer customer;
        [SerializeField] AudioClip[] steps;
        [SerializeField] float interval = 0.42f;

        AudioSource source;
        float next;

        void Awake() => source = GetComponent<AudioSource>();

        void Update()
        {
            if (!customer.IsWalking || Time.time < next || steps.Length == 0) return;
            next = Time.time + interval;
            source.pitch = Random.Range(0.9f, 1.1f);
            source.PlayOneShot(steps[Random.Range(0, steps.Length)], 0.6f);
        }
    }
}
