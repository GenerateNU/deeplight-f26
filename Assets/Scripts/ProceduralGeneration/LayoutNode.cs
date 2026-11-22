using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The record for one placed unit in the layout: its grid position, doors, room type, distance
/// from the start, and room number. Holds data only. distanceFromStart and roomNum are -1
/// until the generator fills them in
/// </summary>
public class LayoutNode
{
    public Vector2Int gridPosition;
    public DoorSet doors;
    public RoomType type;
    public int distanceFromStart = -1;
    public int roomNum = -1;

    /// <summary>
    /// Creates a node with no sides connected and no distance or room number assigned yet
    /// </summary>
    /// <param name="gridPosition">Where the unit sits on the map</param>
    /// <param name="type">The role of the unit</param>
    public LayoutNode(Vector2Int gridPosition, RoomType type)
    {
        this.gridPosition = gridPosition;
        this.type = type;
    }

    /// <summary>
    /// Unlocks doors for this node in the directions where there is a connection
    /// </summary>
    /// <param name="gridPosition">Where the unit sits on the map</param>
    /// <param name="type">The role of the unit</param>
    public List<Direction> GetConnectedDirections()
    {
        List<Direction> connectedDirections = new List<Direction>();
        foreach (Direction dir in Directions.All)
        {
            if (doors.IsConnected(dir))
            {
                connectedDirections.Add(dir);
            }
        }
        //TODO: figure out list of directions with connections 
        return connectedDirections;
    } 
}
