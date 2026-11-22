using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Builds the floor in the scene from a generated layout: places one room prefab per unit, then
/// has each room spawn its contents in placement order. Doors and corridors are not built yet
/// </summary>
public class FloorBuilder : MonoBehaviour
{
    [Header("Layout")]
    [SerializeField] private LayoutParams layoutParams = new LayoutParams();

    [Header("Rooms")]
    [SerializeField] private List<GameObject> roomPrefabs = new List<GameObject>();

    [Header("RNG")]
    [SerializeField] private RngStreamsConfig rngConfig;

    private FloorLayout layout;

    // ----------------------------------- UNITY DEFAULTS ---------------------------------------//

    /// <summary>
    /// Builds the floor when the scene starts.
    /// </summary>
    void Start()
    {
        Build();
    }

    // ----------------------------------------- BUILDING ---------------------------------------//

    /// <summary>
    /// Generates a layout from the LevelGen stream, then places a room for every unit and spawns
    /// each room's contents. Rooms are visited in placement order so they draw from the shared
    /// stream in the same order every run.
    /// </summary>
    public void Build()
    {
        if (rngConfig == null)
        {
            Debug.LogWarning("No RngStreamsConfig assigned.", this);
            return;
        }

        if (roomPrefabs.Count == 0)
        {
            Debug.LogWarning("No room prefabs assigned.", this);
            return;
        }

        RngStream rng = rngConfig.Get(RngStreamsConfig.RngStreamType.LevelGen);
        layout = FloorGenerator.Generate(rng, layoutParams, roomPrefabs.Count);

        foreach (LayoutNode node in layout.placementOrder)
        {
            BuildRoom(node);
        }
    }

    /// <summary>
    /// Places the room prefab for one unit at its world position and has it spawn its contents
    /// </summary>
    /// <param name="node">The unit to build</param>
    private void BuildRoom(LayoutNode node)
    {
        GameObject prefab = roomPrefabs[node.roomNum];
        GameObject room = Instantiate(prefab, GridToWorld(node.gridPosition), Quaternion.identity, transform);

        RoomSpawner spawner = room.GetComponentInChildren<RoomSpawner>();
        if (spawner == null)
        {
            Debug.LogWarning($"{prefab.name} has no RoomSpawner.", room);
            return;
        }

        spawner.Spawn();
    }

    /// <summary>
    /// Converts a grid position to a world position. Each grid position is one whole
    /// node (room plus corridors), so neighboring positions are unitSize apart. Grid x maps to
    /// world x and grid y maps to world z
    /// </summary>
    /// <param name="gridPosition">The grid position of a unit</param>
    /// <returns>
    /// (Vector3) the world position of the unit's center, relative to this object
    /// </returns>
    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        float size = LayoutParams.unitSize;
        return transform.position + new Vector3(gridPosition.x * size, 0f, gridPosition.y * size);
    }
}
