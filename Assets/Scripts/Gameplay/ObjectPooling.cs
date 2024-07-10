using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools; // Different types of pools
    private static Dictionary<string, Queue<GameObject>> poolDictionary; // Pools corresponding to tags
    private static List<Pair<GameObject, string>> spawnedObjects;

    void Awake()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        spawnedObjects = new List<Pair<GameObject, string>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            pool.prefab.SetActive(false);

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab); // copy of prefab
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    private void LateUpdate()
    {
        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            Pair<GameObject, string> pair = spawnedObjects[i];
            GameObject gObject = pair.First;
            string tag = pair.Second;

            if (!gObject.activeInHierarchy) // If the object is not active in the game
            {
                Debug.Log("Put back into the pool");
                DespawnToPool(gObject, tag); // Enqueue it back into the pool
                spawnedObjects.RemoveAt(i);
            }
        }
    }

    private static void DespawnToPool(GameObject gObject, string tag)
    {
        gObject.SetActive(false);
        poolDictionary[tag].Enqueue(gObject); // Add back to pool
    }

    public static GameObject SpawnFromPool(string tag, Vector3 position)
    {
        if (!poolDictionary.ContainsKey(tag)) // if the pool does not contain this key
        {
            // Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        if (poolDictionary[tag].Count == 0) // If the pool is empty
        {
            // Debug.LogWarning("Pool with tag " + tag + " is empty.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.transform.position = position;

        spawnedObjects.Add(new Pair<GameObject, string>(objectToSpawn, tag)); 
        // add to the list of spawned objects

        return objectToSpawn;
    }

    public class Pair<T1, T2>
    {
        public T1 First { get; }
        public T2 Second { get; }

        public Pair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }
    }
}
