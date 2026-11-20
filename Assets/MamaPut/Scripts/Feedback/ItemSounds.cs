using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MamaPut
{
    // Pick-up and knock sounds for anything the player can grab.
    [RequireComponent(typeof(AudioSource))]
    public class ItemSounds : MonoBehaviour
    {
        [SerializeField] AudioClip[] grabClips;
        [SerializeField] AudioClip[] hitClips;
        [SerializeField] float minHitSpeed = 0.4f;
        [SerializeField] float hitCooldown = 0.12f;

        AudioSource source;
        XRGrabInteractable grab;
        float nextHit;

        void Awake()
        {
            source = GetComponent<AudioSource>();
            grab = GetComponent<XRGrabInteractable>();
            if (grab != null) grab.selectEntered.AddListener(OnGrab);
        }

        void OnDestroy()
        {
            if (grab != null) grab.selectEntered.RemoveListener(OnGrab);
        }

        void OnGrab(SelectEnterEventArgs args) => PlayRandom(grabClips, 0.7f);

        void OnCollisionEnter(Collision collision)
        {
            float speed = collision.relativeVelocity.magnitude;
            if (speed < minHitSpeed || Time.time < nextHit) return;
            nextHit = Time.time + hitCooldown;
            PlayRandom(hitClips, Mathf.Clamp01(speed / 2.5f));
        }

        void PlayRandom(AudioClip[] clips, float volume)
        {
            if (clips == null || clips.Length == 0) return;
            source.pitch = Random.Range(0.94f, 1.06f);
            source.PlayOneShot(clips[Random.Range(0, clips.Length)], volume);
        }
    }
}
