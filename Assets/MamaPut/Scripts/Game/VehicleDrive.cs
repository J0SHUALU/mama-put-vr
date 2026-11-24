using UnityEngine;

namespace MamaPut
{
    public class VehicleDrive : MonoBehaviour
    {
        [SerializeField] float speed = 6f;
        [Tooltip("World x where the road starts and ends. The vehicle wraps between them.")]
        [SerializeField] float minX = -30f;
        [SerializeField] float maxX = 30f;

        void Update()
        {
            var p = transform.position + transform.forward * (speed * Time.deltaTime);
            if (p.x > maxX) p.x = minX;
            else if (p.x < minX) p.x = maxX;
            transform.position = p;
        }
    }
}
