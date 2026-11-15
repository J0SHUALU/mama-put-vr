using System.Collections.Generic;
using UnityEngine;

namespace MamaPut
{
    public class ObjectPool<T> where T : Component
    {
        readonly T prefab;
        readonly Transform parent;
        readonly Stack<T> free = new Stack<T>();

        public ObjectPool(T prefab, Transform parent, int prewarm = 0)
        {
            this.prefab = prefab;
            this.parent = parent;
            for (int i = 0; i < prewarm; i++) Release(Object.Instantiate(prefab, parent));
        }

        public T Get(Vector3 position, Quaternion rotation)
        {
            var item = free.Count > 0 ? free.Pop() : Object.Instantiate(prefab, parent);
            item.transform.SetPositionAndRotation(position, rotation);
            item.gameObject.SetActive(true);
            if (item is IPoolable p) p.OnSpawn();
            return item;
        }

        public void Release(T item)
        {
            if (item is IPoolable p) p.OnDespawn();
            item.gameObject.SetActive(false);
            free.Push(item);
        }
    }
}
