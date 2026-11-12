using UnityEngine;

/// <summary>
/// An empty grid position next to a placed unit, plus the direction that reaches it from that unit.
/// Only FloorGenerator keeps these, in its local list, and they are not part of the finished layout
/// </summary>
public struct OpenSlot
{
    /// <summary>
    /// The empty grid position this slot points at
    /// </summary>
    public Vector2Int gridPosition;

    /// <summary>
    /// The direction to step from the placed unit to reach gridPosition
    /// </summary>
    public Direction direction;

    /// <summary>
    /// Creates an open slot
    /// </summary>
    /// <param name="gridPosition">The empty grid position</param>
    /// <param name="direction">The direction from the placed unit that reaches it</param>
    public OpenSlot(Vector2Int gridPosition, Direction direction)
    {
        this.gridPosition = gridPosition;
        this.direction = direction;
    }
}
