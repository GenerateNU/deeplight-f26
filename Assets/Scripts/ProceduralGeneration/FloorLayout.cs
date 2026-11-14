using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The finished map, stored as grid position -> LayoutNode. Handles placing units and connecting
/// neighbors. Never touches the RNG
/// </summary>
public class FloorLayout
{
    
    public LayoutNode startNode;
    public LayoutNode bossNode;

    public readonly Dictionary<Vector2Int, LayoutNode> nodes = new Dictionary<Vector2Int, LayoutNode>();

    /// <summary>
    /// Checks whether a unit is already placed at a grid position
    /// </summary>
    /// <param name="gridPosition">The position to check</param>
    /// <returns>
    /// (bool) true if a unit is placed there
    /// </returns>
    public bool IsOccupied(Vector2Int gridPosition)
    {
        return nodes.ContainsKey(gridPosition);
    }

    /// <summary>
    /// Looks up the unit at a grid position
    /// </summary>
    /// <param name="gridPosition">The position to look at</param>
    /// <param name="node">The unit found there, or null if the position is empty</param>
    /// <returns>
    /// (bool) true if a unit was found
    /// </returns>
    public bool TryGetNode(Vector2Int gridPosition, out LayoutNode node)
    {
        return nodes.TryGetValue(gridPosition, out node);
    }

    /// <summary>
    /// Places a new unit, then connects it to every unit already next to it. Connecting to every
    /// neighbor, not just the one it grew from, is where loops come from
    /// </summary>
    /// <param name="gridPosition">Where to place the unit. Must be empty</param>
    /// <param name="type">The role of the new unit</param>
    /// <returns>
    /// (LayoutNode) the node that was placed
    /// </returns>
    public LayoutNode PlaceNode(Vector2Int gridPosition, RoomType type)
    {
        if (IsOccupied(gridPosition))
        {
            throw new InvalidOperationException($"A unit is already placed at {gridPosition}");
        }

        LayoutNode node = new LayoutNode(gridPosition, type);
        nodes.Add(gridPosition, node);

        if (type == RoomType.Start)
        {
            startNode = node;
        }
        else if (type == RoomType.Boss)
        {
            bossNode = node;
        }

        foreach (Direction dir in Directions.All)
        {
            if (nodes.TryGetValue(dir.GetNeighbor(gridPosition), out LayoutNode neighbor))
            {
                Connect(node, neighbor);
            }
        }

        return node;
    }

    /// <summary>
    /// Closes the matching sides on two touching units, so each one is connected to the other
    /// </summary>
    /// <param name="a">One of the two units</param>
    /// <param name="b">The unit next to it. Must touch a on one side</param>
    public void Connect(LayoutNode a, LayoutNode b)
    {
        foreach (Direction dir in Directions.All)
        {
            if (dir.GetNeighbor(a.gridPosition) == b.gridPosition)
            {
                a.doors.SetOpen(dir, false);
                b.doors.SetOpen(dir.Opposite(), false);
                return;
            }
        }

        throw new ArgumentException($"{a.gridPosition} and {b.gridPosition} are not next to each other");
    }
}
