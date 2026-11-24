using System.Collections.Generic;
using UnityEngine;

namespace MamaPut
{
    public class CustomerWallet : MonoBehaviour
    {
        [SerializeField] NairaNote[] notePrefabs;

        readonly Dictionary<int, ObjectPool<NairaNote>> pools = new Dictionary<int, ObjectPool<NairaNote>>();

        public static CustomerWallet Instance { get; private set; }

        void Awake()
        {
            Instance = this;
            foreach (var prefab in notePrefabs) pools[prefab.Value] = new ObjectPool<NairaNote>(prefab, transform, 2);
        }

        public List<int> ChooseNotes(int total, bool exact)
        {
            var values = new List<int>(pools.Keys);
            values.Sort();
            var result = new List<int>();

            if (!exact)
            {
                foreach (var v in values)
                    if (v > total) { result.Add(v); return result; }
            }

            int left = total;
            for (int i = values.Count - 1; i >= 0 && left > 0; i--)
                while (left >= values[i]) { result.Add(values[i]); left -= values[i]; }

            if (left > 0 || !exact) result.Add(values[0]);
            return result;
        }

        public NairaNote Spawn(int value, Transform hand, int index)
        {
            int k = index - 1;
            var offset = hand.right * (0.09f * k) + Vector3.up * (0.03f * k);
            var note = pools[value].Get(hand.position + offset, hand.rotation);
            note.ReturnToPool = pools[value].Release;
            note.HoldStill();
            return note;
        }
    }
}
