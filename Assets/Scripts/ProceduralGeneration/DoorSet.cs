using System;

/// <summary>
/// One connected flag per Direction. A side is connected when a unit is placed next to it and
/// the two are linked. A side that is not connected is empty, so a room could still go there
/// (the same spot an OpenSlot points at)
/// </summary>
/// <remarks>
/// This is a struct, so SetConnected only changes the copy it is called on. Call it on a field
/// (node.doors.SetConnected(...)), not on a property or a value returned from a method.
/// A default DoorSet has no sides connected, which is how a freshly placed unit starts
/// </remarks>
public struct DoorSet
{
    private bool northConnected;
    private bool eastConnected;
    private bool southConnected;
    private bool westConnected;

    /// <summary>
    /// How many sides are connected to a neighboring unit
    /// </summary>
    public int ConnectedCount
    {
        get
        {
            int count = 0;
            foreach (Direction dir in Directions.All)
            {
                if (IsConnected(dir))
                {
                    count++;
                }
            }
            return count;
        }
    }

    /// <summary>
    /// Checks whether a side is connected to a neighboring unit
    /// </summary>
    /// <param name="dir">The side to check</param>
    /// <returns>
    /// (bool) true if a unit is placed and connected on that side.
    /// false if nothing is there, so a room could still be placed there
    /// </returns>
    public bool IsConnected(Direction dir)
    {
        switch (dir)
        {
            case Direction.North:
                return northConnected;
            case Direction.East:
                return eastConnected;
            case Direction.South:
                return southConnected;
            case Direction.West:
                return westConnected;
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
    }

    /// <summary>
    /// Marks a side as connected or not
    /// </summary>
    /// <param name="dir">The side to change</param>
    /// <param name="connected">True if a unit is connected on that side, false if it is empty</param>
    public void SetConnected(Direction dir, bool connected)
    {
        switch (dir)
        {
            case Direction.North:
                northConnected = connected;
                break;
            case Direction.East:
                eastConnected = connected;
                break;
            case Direction.South:
                southConnected = connected;
                break;
            case Direction.West:
                westConnected = connected;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
    }
}
