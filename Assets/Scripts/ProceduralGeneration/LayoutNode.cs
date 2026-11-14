using UnityEngine;

/// <summary>
/// The record for one placed unit in the layout: its grid position, doors, room type, distance
/// from the start, room number, and seed. Holds data only. distanceFromStart and roomNum are -1
/// until the generator fills them in
/// </summary>
public class LayoutNode
{
    public Vector2Int gridPosition;
    public DoorSet doors;
    public RoomType type;
    public int distanceFromStart = -1;
    public int roomNum = -1;
    public int seed;

    /// <summary>
    /// Creates a node with every side open and no distance, room number, or seed assigned yet
    /// </summary>
    /// <param name="gridPosition">Where the unit sits on the map</param>
    /// <param name="type">The role of the unit</param>
    public LayoutNode(Vector2Int gridPosition, RoomType type)
    {
        this.gridPosition = gridPosition;
        this.type = type;
    }
}
