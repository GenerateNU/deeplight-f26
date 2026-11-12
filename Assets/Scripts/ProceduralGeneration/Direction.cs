using System;
using UnityEngine;

/// <summary>
/// The four directions a unit can connect in. Used for door sides, open slots, and
/// stepping between neighboring grid positions
/// </summary>
public enum Direction
{
    North,
    East,
    South,
    West
}

/// <summary>
/// Helpers for Direction: the opposite side, the grid step it represents, and a list of all four
/// </summary>
public static class Directions
{
    /// <summary>
    /// Every direction, in a fixed order. Use this to loop over the four sides so iteration
    /// order is the same every run
    /// </summary>
    public static readonly Direction[] All =
    {
        Direction.North,
        Direction.East,
        Direction.South,
        Direction.West
    };

    /// <summary>
    /// Gets the direction facing back the other way
    /// </summary>
    /// <param name="dir">The direction to flip</param>
    /// <returns>
    /// (Direction) the opposite of dir, so North gives South and East gives West
    /// </returns>
    public static Direction Opposite(this Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return Direction.South;
            case Direction.East: return Direction.West;
            case Direction.South: return Direction.North;
            case Direction.West: return Direction.East;
            default: throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
    }

    /// <summary>
    /// Gets the grid position of the neighbor next to a position in this direction
    /// </summary>
    /// <param name="dir">The direction to step in</param>
    /// <param name="from">The grid position to start from</param>
    /// <returns>
    /// (Vector2Int) the position one step away. North adds (0, 1), East adds (1, 0),
    /// South adds (0, -1), West adds (-1, 0)
    /// </returns>
    public static Vector2Int GetNeighbor(this Direction dir, Vector2Int from)
    {
        switch (dir)
        {
            case Direction.North: return from + Vector2Int.up;
            case Direction.East: return from + Vector2Int.right;
            case Direction.South: return from + Vector2Int.down;
            case Direction.West: return from + Vector2Int.left;
            default: throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
    }
}
