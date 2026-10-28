using UnityEngine;

/// <summary>
/// Spawn points can be of type Enemy or Item (can be added to later on)
/// </summary>
public enum SpawnType
{
    Enemy,
    Item
}

/// <summary>
/// A spawn point for a game object of spawnType
/// </summary>
/// <remarks>
/// Attach to an empty game object in a room. Game object must be a child object of the object with the RoomSpawner script attached to it
/// </remarks>
public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private SpawnType spawnType = SpawnType.Enemy;
 
    public SpawnType Type => spawnType;
}
