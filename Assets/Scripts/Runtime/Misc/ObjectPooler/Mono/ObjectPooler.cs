using System.Collections.Generic;
using UnityEngine;
using static Pool;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] List<Pool> pools;

    Dictionary<PoolTag, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        poolDictionary = new Dictionary<PoolTag, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject GetPooledObject(PoolTag tag, Vector3 position, Quaternion rotation, Transform parent)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        if (poolDictionary[tag].Count == 0)
        {
            AddObjectsToPool(tag, 10);
        }

        GameObject obj = poolDictionary[tag].Dequeue();
        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.transform.SetParent(parent, true);

        IPoolable poolable = obj.GetComponent<IPoolable>();
        if (poolable != null)
        {
            poolable.OnObjectSpawn();
        }

        return obj;
    }

    public void ReturnObjectToPool(PoolTag tag, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return;
        }

        IPoolable poolable = obj.GetComponent<IPoolable>();
        if (poolable != null)
        {
            poolable.OnObjectReturn();
        }

        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
    }

    private void AddObjectsToPool(PoolTag tag, int count)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return;
        }

        Pool pool = pools.Find(p => p.tag == tag);
        if (pool == null) return;

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(pool.prefab);
            IPoolable poolableComponent = obj.GetComponent<IPoolable>();
            if (poolableComponent == null)
            {
                Debug.LogError($"Prefab for pool {tag} does not have an IPoolable component.");
                continue;
            }
            obj.SetActive(false);
            poolDictionary[tag].Enqueue(obj);
        }
    }
}
