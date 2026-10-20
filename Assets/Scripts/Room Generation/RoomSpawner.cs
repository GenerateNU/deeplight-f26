using UnityEngine;
using System.Collections.Generic;

public class RoomSpawner : MonoBehaviour
{
    [Header("Room Data File")]
    [SerializeField] private RoomData roomData;

    [Header("Seed Information (Optional)")]
    [SerializeField] private int seed = 0;
    [SerializeField] private bool useSeed = false;

    [Header("Spawning")]
    [SerializeField] private KeyCode debugSpawnKey = KeyCode.R;
    [SerializeField] private bool debugKeyEnabled = false;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    private System.Random rng;

    // ----------------------------------- UNITY DEFAULTS ---------------------------------------//

    /// <summary>
    /// Runs once when script is loaded.
    /// </summary>
    void Awake()
    {
        CreateRng();
    }

    /// <summary>
    /// Runs at the start of the scene.
    /// </summary>
    void Start()
    {
        Spawn();
    }

    /// <summary>
    /// Runs once per frame.
    /// </summary>
    void Update()
    {
        if(debugKeyEnabled && Input.GetKeyDown(debugSpawnKey)) Spawn();
    }

    // ---------------------------------------- RNG SETUP --------------------------------------- //

    /// <summary>
    /// Creates an rng, can be based off of a seeded integer.
    /// </summary>
    private void CreateRng()
    {
        rng = useSeed ? new System.Random(seed) : new System.Random();
    }

    /// <summary>
    /// Sets the integer seed.
    /// </summary>
    /// <param name="newSeed"> The value of the new seed once set.
    private void SetSeed(int newSeed)
    {
        useSeed = true;
        seed = newSeed;
        CreateRng();
    }

    // ---------------------------------------- ENEMY SPAWNING -----------------------------------//

    /// <summary>
    /// Checks if there is room data, an rng (creates one if not). Then Despawns any entities still in
    /// the spawn pool, and spawns new entities of each respective category (Enemy, Item) at their randomly
    /// selected spawn points.
    /// </summary>
    public void Spawn()
    {
        if (roomData == null)
        {
            Debug.LogWarning("No RoomData assigned.", this);
            return;
        }
 
        if (useSeed || rng == null) CreateRng();
 
        Despawn();
 
        SpawnPoint[] points = GetComponentsInChildren<SpawnPoint>();
 
        SpawnCategory(points, SpawnType.Enemy, roomData.RollEnemyCount(rng), roomData.PickEnemyPrefab);
        SpawnCategory(points, SpawnType.Item, roomData.RollItemCount(rng), roomData.PickItemPrefab);
    }

    /// <summary>
    /// Despawns all enemies left in spawnedObjects.
    /// </summary>
    public void Despawn()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedObjects.Clear();
    }

    /// <summary>
    /// Finds all candidate spawn points of the selected type, then randomly spawns entities 
    /// at those spawn points from the room data's spawn table until there are no more 
    /// available candidate spawn points, or there are no more available entities.
    /// </summary>
    /// <param name="allPoints"> All spawn points in the room.
    /// <param name="type"> The type of entities to spawn.
    /// <param name="desiredCount"> The number of entities to spawn in the room.
    /// <param name="pickPrefab"> A funtion call to the desired entity selection function in the 
    /// room data file.
    private void SpawnCategory(
        SpawnPoint[] allPoints,
        SpawnType type,
        int desiredCount,
        System.Func<System.Random, GameObject> pickPrefab)
    {
        var candidates = new List<SpawnPoint>();
        foreach (SpawnPoint point in allPoints)
        {
            if (point.Type == type) candidates.Add(point);
        }
 
        if (desiredCount > candidates.Count)
        {
            Debug.LogWarning("Less candidates available than the desired count.", this);
            desiredCount = candidates.Count;
        }
 
        for (int i = 0; i < desiredCount; i++)
        {
            int j = rng.Next(i, candidates.Count);
            (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
 
            GameObject prefab = pickPrefab(rng);
            if (prefab == null)
            {
                Debug.LogWarning($"{name}: {type} spawn table has no valid entries.", this);
                return;
            }
 
            Transform point = candidates[i].transform;
            GameObject instance = Instantiate(prefab, point.position, point.rotation, transform);
            spawnedObjects.Add(instance);
        }
    }
}
