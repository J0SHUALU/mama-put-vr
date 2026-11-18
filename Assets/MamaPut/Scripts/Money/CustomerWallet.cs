using System.Collections.Generic;
using UnityEngine;

namespace MamaPut
{
    // Picks which notes a customer pays with and spawns them in their hand.
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

        // Exact money: the fewest notes that add up to the total.
        // Otherwise: the smallest single note that covers it, so the player has to give change.
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

            // Round up with the smallest note when the total doesn't break down evenly,
            // or when the order is bigger than our biggest note and change is still wanted.
            if (left > 0 || !exact) result.Add(values[0]);
            return result;
        }

        public NairaNote Spawn(int value, Transform hand, int index)
        {
            var offset = hand.right * (0.04f * index);
            var note = pools[value].Get(hand.position + offset, hand.rotation);
            note.ReturnToPool = pools[value].Release;
            note.HoldStill();
            return note;
        }
    }
}
