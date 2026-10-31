using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class is an ObjectPool that holds multiple Objects, allowing them to be extracted.
/// Objects are initially set inactive in the scene. 
/// </summary>
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [Header("Object Pool Settings")]
    [SerializeField] private int startingCapacity = 50;
    [SerializeField] private int maxSize = 500;
    [SerializeField] private List<GameObject> objectPrefabs = new List<GameObject>(); // Can later be turned into a list of projectile types for example

    private Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>(); // Can later be turned into a dictionary of projectile types for example

    /// <summary>
    /// A method that is called when the script is loaded, before Start.
    /// Used to initialize the ObjectPool.
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FillPool();
    }

    /// <summary>
    /// A method that chooses the next Pooled Object to extract from the ObjectPool. 
    /// </summary>
    /// <returns>The next available Pooled Object.</returns>
    public GameObject GetPooledObject(GameObject prefab)
    {
        if (prefab == null)
        {
            return null;
        }

        List<GameObject> pool = GetPool(prefab);

        for (int i = pool.Count - 1; i >= 0; i--)
        {
            if (pool[i] == null)
            {
                pool.RemoveAt(i);
                continue;
            }

            if (!pool[i].activeSelf)
            {
                pool[i].SetActive(true);
                return pool[i];
            }
        }

        GameObject obj = Instantiate(prefab, GetPoolParent(prefab));
        pool.Add(obj);
        return obj;
    }

    /// <summary>
    /// A method that returns a pooled object to the ObjectPool.
    /// </summary>
    /// <param name="pooledObject"></param>
    public void ReturnPooledObject(GameObject pooledObject)
    {
        if (pooledObject == null)
            return;
        if (!pooledObject.activeSelf)
            return;

        pooledObject.SetActive(false);
    }

    /// <summary>
    /// A method that fills the ObjectPool by spawning new objects.
    /// Can be expanded to fill the pool for multiple types of objects.
    /// </summary>
    private void FillPool()
    {
        foreach (GameObject prefab in objectPrefabs)
        {
            List<GameObject> pool = GetPool(prefab);

            for (int i = 0; i < startingCapacity; i++)
            {
                GameObject obj = Instantiate(prefab, GetPoolParent(prefab));
                obj.SetActive(false);
                pool.Add(obj);
            }
        }
    }

    /// <summary>
    /// A method that counts the number of inactive objects in the ObjectPool.
    /// i.e. the number of available objects to take out of the pool. 
    /// </summary>
    /// <returns>The count of inactive objects in ObjectPool.</returns>
    private int InactiveCount(GameObject prefab)
    {
        int count = 0;

        foreach (GameObject obj in GetPool(prefab))
        {
            if (!obj.activeSelf)
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// A method that searches the dictionary for a specific object and returns that object pool.
    /// If the object searched for doesn't have a pool, it creates one. 
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns>A specific object pool.</returns>
    private List<GameObject> GetPool(GameObject prefab)
    {
        if (!pooledObjects.TryGetValue(prefab, out List<GameObject> pool))
        {
            pool = new List<GameObject>();
            pooledObjects.Add(prefab, pool);
        }
        return pool;
    }

    /// <summary>
    /// A method that finds or creates a parent for a specific object pool. 
    /// This allows the hierarchy in the Unity UI to look cleaner.
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    private Transform GetPoolParent(GameObject prefab)
    {
        Transform parent = transform.Find(prefab.name);
        if (parent == null)
        {
            parent = new GameObject(prefab.name).transform;
            parent.SetParent(transform);
        }
        return parent;
    }
}
