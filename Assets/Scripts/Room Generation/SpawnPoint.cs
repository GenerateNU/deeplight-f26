using UnityEngine;

public enum SpawnType
{
    Enemy,
    Item
}

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private SpawnType spawnType = SpawnType.Enemy;
 
    public SpawnType Type => spawnType;
}
