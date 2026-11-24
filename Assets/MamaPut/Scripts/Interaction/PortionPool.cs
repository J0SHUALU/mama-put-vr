using System.Collections.Generic;
using UnityEngine;

namespace MamaPut
{
    public static class PortionPool
    {
        static readonly Dictionary<GameObject, Stack<GameObject>> free = new Dictionary<GameObject, Stack<GameObject>>();
        static readonly Dictionary<GameObject, GameObject> owner = new Dictionary<GameObject, GameObject>();

        public static GameObject Get(DishData dish, Transform parent)
        {
            if (!free.TryGetValue(dish.portionPrefab, out var stack))
                free[dish.portionPrefab] = stack = new Stack<GameObject>();

            GameObject go;
            if (stack.Count > 0)
            {
                go = stack.Pop();
                go.transform.SetParent(parent, false);
            }
            else
            {
                go = Object.Instantiate(dish.portionPrefab, parent, false);
                owner[go] = dish.portionPrefab;
            }
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.SetActive(true);
            return go;
        }

        public static void Release(GameObject portion)
        {
            if (portion == null) return;
            portion.SetActive(false);
            portion.transform.SetParent(null, false);
            if (owner.TryGetValue(portion, out var prefab)) free[prefab].Push(portion);
            else Object.Destroy(portion);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset()
        {
            free.Clear();
            owner.Clear();
        }
    }
}
