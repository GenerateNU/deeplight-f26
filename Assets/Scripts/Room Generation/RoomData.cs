using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SpawnEntry
{
    public GameObject prefab;
    [Min(0f)] public float weight = 1f;
}

[CreateAssetMenu(fileName = "Room Datafile", menuName = "ScriptableObjects/New RoomData")]
public class RoomData : ScriptableObject
{
    [Header("Room Identification")]
    private string roomName;
    private string roomID;

    [Header("Enemies")]
    [SerializeField] private int minEnemyCount = 0;
    [SerializeField] private int maxEnemyCount = 1;
    [SerializeField] private List<SpawnEntry> enemySpawnTable;

    [Header("Items")]
    [SerializeField] private int minItemCount = 0;
    [SerializeField] private int maxItemCount = 1;
    [SerializeField] private List<SpawnEntry> itemSpawnTable;

    // -------------------------------- UNITY DEFAULTS -------------------------------- //

    /// <summary>
    /// Runs when any change is made to the ScriptableObject in the editor.
    /// </summary>
    private void OnValidate()
    {
        //min enemy count will be at least 0, and max enemy count will be at least min enemy count
        minEnemyCount = Mathf.Max(0, minEnemyCount);
        maxEnemyCount = Mathf.Max(minEnemyCount, maxEnemyCount);
 
        //min item count will be at least 0, and max item count will be at least min item count
        minItemCount = Mathf.Max(0, minItemCount);
        maxItemCount = Mathf.Max(minItemCount, maxItemCount);

        //if any non-enemy prefab is placed in the enemy spawn table, it's weight will be defaulted to 0
        foreach (SpawnEntry entry in enemySpawnTable)
        {
            if (!entry.prefab.CompareTag("Enemy"))
            {
                entry.weight = 0f;
            }
        }

        //if any non-item prefab is placed in the item spawn table, it's weight will be defaulted to 0
        foreach (SpawnEntry entry in itemSpawnTable)
        {
            if (!entry.prefab.CompareTag("Item"))
            {
                entry.weight = 0f;
            }
        }
    }

    // -------------------------------- SPWAN COUNTS -------------------------------- //

    /// <summary>
    /// Random enemy count in [min, max] inclusive.
    /// </summary>
    /// <param name="rng"> random number generator
    /// <returns>
    /// (int) a random integer number of enemies to spawn
    /// </returns>
    public int RollEnemyCount(System.Random rng) => rng.Next(minEnemyCount, maxEnemyCount + 1);
 
    /// <summary>
    /// Random item count in [min, max] inclusive.
    /// </summary>
    /// <param name="rng"> random number generator
    /// <returns>
    /// (int) a random integer number of items to spawn
    /// </returns>
    public int RollItemCount(System.Random rng) => rng.Next(minItemCount, maxItemCount + 1);

    // -------------------------------- SELECTING ENTITIES -------------------------------- //
 
    public GameObject PickEnemyPrefab(System.Random rng) => PickWeighted(enemySpawnTable, rng);
 
    public GameObject PickItemPrefab(System.Random rng) => PickWeighted(itemSpawnTable, rng);
 
    /// <summary>
    /// Weighted random pick. Entries with a null prefab or weight of 0f are ignored.
    /// Returns null if nothing is pickable.
    /// </summary>
    /// <param name="table"> list of entries that could be spawned
    /// <param name="rng"> random number generator
    /// <returns>
    /// (GameObject) an entry picked at random from table
    /// </returns>
    public static GameObject PickWeighted(List<SpawnEntry> table, System.Random rng)
    {
        if (table == null) return null;
 
        double total = 0.0;
        foreach (SpawnEntry entry in table)
        {
            if (entry != null && entry.prefab != null && entry.weight > 0f)
                total += entry.weight;
        }
 
        if (total <= 0.0) return null;
 
        double roll = rng.NextDouble() * total;
        GameObject lastValid = null;
 
        foreach (SpawnEntry entry in table)
        {
            if (entry == null || entry.prefab == null || entry.weight <= 0f) continue;
 
            lastValid = entry.prefab;
            roll -= entry.weight;
            if (roll < 0.0) return entry.prefab;
        }
 
        return lastValid;
    }
}
