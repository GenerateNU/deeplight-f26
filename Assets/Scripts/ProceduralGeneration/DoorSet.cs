using System;

/// <summary>
/// One open or closed flag per Direction. A side is open when nothing is placed next to it, so a
/// room could still go there (same meaning as OpenSlot). A side is closed when a unit is placed
/// next to it and the two are connected
/// </summary>
/// <remarks>
/// This is a struct, so SetOpen only changes the copy it is called on. Call it on a field
/// (node.doors.SetOpen(...)), not on a property or a value returned from a method.
/// A default DoorSet has every side open, which is how a freshly placed unit starts
/// </remarks>
public struct DoorSet
{
    // Stored as "closed" so that a default (all false) DoorSet has every side open
    private bool northClosed;
    private bool eastClosed;
    private bool southClosed;
    private bool westClosed;

    /// <summary>
    /// How many sides are currently open, meaning no unit is connected there
    /// </summary>
    public int OpenCount
    {
        get
        {
            int count = 0;
            foreach (Direction dir in Directions.All)
            {
                if (IsOpen(dir))
                {
                    count++;
                }
            }
            return count;
        }
    }

    /// <summary>
    /// Checks whether a side is open
    /// </summary>
    /// <param name="dir">The side to check</param>
    /// <returns>
    /// (bool) true if nothing is connected on that side, so a room could be placed there.
    /// false if a unit is already connected there
    /// </returns>
    public bool IsOpen(Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return !northClosed;
            case Direction.East: return !eastClosed;
            case Direction.South: return !southClosed;
            case Direction.West: return !westClosed;
            default: throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
    }

    /// <summary>
    /// Opens or closes a side
    /// </summary>
    /// <param name="dir">The side to change</param>
    /// <param name="open">True to mark the side open (nothing connected), false to mark it
    /// closed (a unit is connected there)</param>
    public void SetOpen(Direction dir, bool open)
    {
        switch (dir)
        {
            case Direction.North: northClosed = !open; break;
            case Direction.East: eastClosed = !open; break;
            case Direction.South: southClosed = !open; break;
            case Direction.West: westClosed = !open; break;
            default: throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
    }
}
