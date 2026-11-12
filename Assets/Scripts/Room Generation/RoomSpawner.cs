using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Functionality for spawning entities in rooms using a roomData file
/// </summary>
public class RoomSpawner : MonoBehaviour
{
    [Header("Room Data File")]
    [SerializeField] private RoomData roomData;

    [Header("RNG")]
    [SerializeField] private RngStreamsConfig rngConfig;

    [Header("Spawning")]
    [SerializeField] private KeyCode debugSpawnKey = KeyCode.R;
    [SerializeField] private bool debugKeyEnabled = false;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    private RngStream Rng => rngConfig.Get(RngStreamsConfig.RngStreamType.LevelGen);

    // ----------------------------------- UNITY DEFAULTS ---------------------------------------//

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
        if (debugKeyEnabled && Input.GetKeyDown(debugSpawnKey))
        {
            Spawn();
        }
    }

    // ---------------------------------------- ENEMY SPAWNING -----------------------------------//

    /// <summary>
    /// Checks if there is room data, an rng. Then Despawns any entities still in
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

        if (rngConfig == null)
        {
            Debug.LogWarning("No RngStreamsConfig assigned.", this);
            return;
        }

        Despawn();

        SpawnPoint[] points = GetComponentsInChildren<SpawnPoint>();
        RngStream rng = Rng;

        SpawnCategory(points, SpawnType.Enemy, roomData.RollEnemyCount(rng), roomData.PickEnemyPrefab, rng);
        SpawnCategory(points, SpawnType.Item, roomData.RollItemCount(rng), roomData.PickItemPrefab, rng);
    }

    /// <summary>
    /// Despawns all enemies left in spawnedObjects.
    /// </summary>
    public void Despawn()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
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
        System.Func<RngStream, GameObject> pickPrefab,
        RngStream rng)
    {
        var candidates = new List<SpawnPoint>();
        foreach (SpawnPoint point in allPoints)
        {
            if (point.Type == type)
            {
                candidates.Add(point);
            }
        }

        if (desiredCount > candidates.Count)
        {
            Debug.LogWarning("Less candidates available than the desired count.", this);
            desiredCount = candidates.Count;
        }

        for (int i = 0; i < desiredCount; i++)
        {
            int j = rng.Range(i, candidates.Count);
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
