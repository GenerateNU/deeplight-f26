using System;

/// <summary>
/// One open or closed flag per Direction, describing which sides of a unit have a door
/// </summary>
/// <remarks>
/// This is a struct, so Set only changes the copy it is called on. Call it on a field
/// (node.doors.Set(...)), not on a property or a value returned from a method
/// </remarks>
public struct DoorSet
{
    private bool north;
    private bool east;
    private bool south;
    private bool west;

    /// <summary>
    /// How many sides currently have a door
    /// </summary>
    public int Count
    {
        get
        {
            int count = 0;
            foreach (Direction dir in Directions.All)
            {
                if (Has(dir))
                {
                    count++;
                }
            }
            return count;
        }
    }

    /// <summary>
    /// Checks whether a side has a door
    /// </summary>
    /// <param name="dir">The side to check</param>
    /// <returns>
    /// (bool) true if the door on that side is open
    /// </returns>
    public bool Has(Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return north;
            case Direction.East: return east;
            case Direction.South: return south;
            case Direction.West: return west;
            default: throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
    }

    /// <summary>
    /// Opens or closes the door on a side
    /// </summary>
    /// <param name="dir">The side to change</param>
    /// <param name="open">True to open the door, false to close it</param>
    public void Set(Direction dir, bool open)
    {
        switch (dir)
        {
            case Direction.North: north = open; break;
            case Direction.East: east = open; break;
            case Direction.South: south = open; break;
            case Direction.West: west = open; break;
            default: throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
    }
}
