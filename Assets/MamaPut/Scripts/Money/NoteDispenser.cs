using UnityEngine;

namespace MamaPut
{
    public class NoteDispenser : MonoBehaviour
    {
        [SerializeField] NairaNote notePrefab;
        [SerializeField] Transform spawnPoint;
        [SerializeField] float takenDistance = 0.1f;

        ObjectPool<NairaNote> pool;
        NairaNote current;

        public int Denomination => notePrefab.Value;

        void Awake() => pool = new ObjectPool<NairaNote>(notePrefab, transform, 4);

        void Start() => SpawnNext();

        void Update()
        {
            if (current == null) return;
            if (Vector3.Distance(current.transform.position, spawnPoint.position) > takenDistance) SpawnNext();
        }

        void SpawnNext()
        {
            current = pool.Get(spawnPoint.position, spawnPoint.rotation);
            current.ReturnToPool = pool.Release;
        }
    }
}
