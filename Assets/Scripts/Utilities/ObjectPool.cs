using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class is an ObjectPool that holds multiple of the same GameObject, allowing them to be extracted.
/// Objects are initially set inactive in the scene. 
/// </summary>
public class ObjectPool : MonoBehaviour
{
    [Header("Object Pool Settings")]
    public static ObjectPool instance;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int amountToPool; 

    private List<GameObject> pooledObjects = new List<GameObject>();

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
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = Instantiate(projectilePrefab, transform);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    /// <summary>
    /// A method that chooses the next Pooled Object to extract from the ObjectPool. 
    /// </summary>
    /// <returns>The next available Pooled Object.</returns>
    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        return null;
    }
}
