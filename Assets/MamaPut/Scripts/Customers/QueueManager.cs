using System.Collections.Generic;
using UnityEngine;

namespace MamaPut
{
    public class QueueManager : MonoBehaviour
    {
        [Tooltip("Index 0 is the spot at the counter. The rest go back from there.")]
        [SerializeField] Transform[] spots;
        [SerializeField] Transform stallPoint;

        readonly List<Customer> line = new List<Customer>();

        public Vector3 StallPoint => stallPoint.position;
        public int Count => line.Count;

        public void Join(Customer c)
        {
            if (!line.Contains(c)) line.Add(c);
            Refresh();
        }

        public void Leave(Customer c)
        {
            if (line.Remove(c)) Refresh();
        }

        public bool IsAtCounter(Customer c) => line.Count > 0 && line[0] == c;

        void Refresh()
        {
            for (int i = 0; i < line.Count; i++)
                line[i].WalkTo(spots[Mathf.Min(i, spots.Length - 1)].position);
        }
    }
}
