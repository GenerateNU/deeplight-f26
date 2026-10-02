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
    [SerializeField] private GameObject objectPrefab; // Can later be turned into a list of projectile types for example

    private List<GameObject> pooledObjects = new List<GameObject>(); // Can later be turned into a dictionary of projectile types for example

    /// <summary>
    /// A method that is called when the script is loaded, before Start.
    /// Used to initialize the ObjectPool.
    /// </summary>
    private void Awake()
    {
        if(instance == null)
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
    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeSelf)
            {
                pooledObjects[i].SetActive(true);

                return pooledObjects[i];
            }
        }

        // if there are no more objects left in the pool, create more
        GameObject obj = Instantiate(objectPrefab, transform);
        pooledObjects.Add(obj);
        return obj; 
    }

    /// <summary>
    /// A method that returns a pooled object to the ObjectPool.
    /// </summary>
    /// <param name="pooledObject"></param>
    public void ReturnPooledObject(GameObject pooledObject)
    {
        // if it's already inactive 
        if (!pooledObject.activeSelf)
        {
            return;
        }

        if (InactiveCount() >= maxSize)
        {
            pooledObjects.Remove(pooledObject);
            Destroy(pooledObject);
            return;
        }

        pooledObject.SetActive(false);
    }

    /// <summary>
    /// A method that fills the ObjectPool by spawning new objects.
    /// Can be expanded to fill the pool for multiple types of objects.
    /// </summary>
    private void FillPool()
    {
        for (int i = 0; i < startingCapacity; i++)
        {
            GameObject obj = Instantiate(objectPrefab, transform);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    /// <summary>
    /// A method that counts the number of inactive objects in the ObjectPool.
    /// i.e. the number of available objects to take out of the pool. 
    /// </summary>
    /// <returns>The count of inactive objects in ObjectPool.</returns>
    private int InactiveCount()
    {
        int count = 0; 

        foreach (GameObject obj in pooledObjects)
        {
            if(!obj.activeSelf)
            {
                count++;
            }
        }

        return count;
    }
}