using UnityEngine;

namespace MamaPut
{
    public class PlateStack : MonoBehaviour
    {
        [SerializeField] Plate platePrefab;
        [SerializeField] Transform spawnPoint;
        [SerializeField] int prewarm = 8;
        [SerializeField] float takenDistance = 0.12f;

        ObjectPool<Plate> pool;
        Plate current;

        void Awake() => pool = new ObjectPool<Plate>(platePrefab, transform, prewarm);

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
