using System.Collections.Generic;
using UnityEditor.Rendering;
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

    public List<Pool> pools; // các loại pools
    private static Dictionary<string, Queue<GameObject>> poolDictionary; // pool ứng với tag
    private static List<Pair<GameObject,string>> spanwedObjects;

    void Awake()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        spanwedObjects = new List<Pair<GameObject,string>>();

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

    private void Update() {
        for (int i=0; i<spanwedObjects.Count; i++){
            Pair<GameObject, string> pair = spanwedObjects[i];
            GameObject gObject = pair.First;
            string tag = pair.Second;

            if (!gObject.scene.IsValid()){ // nếu đang không dùng
                despawnToPool(gObject, tag); // bỏ lại vào pool

                spanwedObjects.Remove(pair);
            }
        }
    }

    private static void despawnToPool(GameObject gObject, string tag){
        gObject.SetActive(false);
        poolDictionary[tag].Enqueue(gObject); // thêm lại vào pool
    }

    public static GameObject spawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag)) // không chứa key này
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        spanwedObjects.Add(new Pair<GameObject, string>(objectToSpawn, tag)); // thêm vào danh sách những object đã spawn

        // poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}