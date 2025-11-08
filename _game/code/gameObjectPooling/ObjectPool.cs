using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    public class ObjectPool<T> where T : Component
    {
        private T prefab;
        private Transform parent;
        private List<T> pool = new List<T>();

        public ObjectPool(T prefab, int initialSize, Transform parent = null)
        {
            this.prefab = prefab;
            this.parent = parent;

            for (int i = 0; i < initialSize; i++)
            {
                CreateNewInstance();
            }
        }

        private T CreateNewInstance()
        {
            T newObject = Object.Instantiate(prefab, parent);
            newObject.gameObject.SetActive(false);
            pool.Add(newObject);
            return newObject;
        }

        public T Get(Vector3 position, Quaternion rotation)
        {
            foreach (T item in pool)
            {
                if (item == null)
                {
                    continue;
                }

                if (!item.gameObject.activeInHierarchy)
                {
                    item.transform.position = position;
                    item.transform.rotation = rotation;
                    item.gameObject.SetActive(true);
                    return item;
                }
            }

            T newObject = CreateNewInstance();
            newObject.transform.position = position;
            newObject.transform.rotation = rotation;
            newObject.gameObject.SetActive(true);
            return newObject;
        }

        public void ReturnToPool(T item)
        {
            if (item != null)
            {
                item.gameObject.SetActive(false);
            }
        }

        public void Clear()
        {
            foreach (T item in pool)
            {
                if (item != null)
                {
                    Object.Destroy(item.gameObject);
                }
            }
            pool.Clear();
        }
    }
}