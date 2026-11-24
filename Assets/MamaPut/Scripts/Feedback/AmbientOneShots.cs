using UnityEngine;

namespace MamaPut
{
    public class AmbientOneShots : MonoBehaviour
    {
        [SerializeField] AudioSource[] voices;
        [SerializeField] AudioClip[] clips;
        [SerializeField] Transform[] points;
        [SerializeField] Vector2 interval = new Vector2(6f, 16f);

        float next;
        int voice;

        void Start() => next = Time.time + Random.Range(interval.x, interval.y);

        void Update()
        {
            if (Time.time < next || clips.Length == 0 || voices.Length == 0) return;
            next = Time.time + Random.Range(interval.x, interval.y);
            var source = voices[voice];
            voice = (voice + 1) % voices.Length;
            if (points.Length > 0) source.transform.position = points[Random.Range(0, points.Length)].position;
            source.pitch = Random.Range(0.92f, 1.08f);
            source.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        }
    }
}
