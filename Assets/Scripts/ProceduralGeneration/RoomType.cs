/// <summary>
/// The role of a unit in the layout. The start unit is where the player spawns. Boss is on hold
/// </summary>
public enum RoomType
{
    /// <summary>
    /// The unit at (0, 0) where the player spawns
    /// </summary>
    Start,

    /// <summary>
    /// A regular unit filled from the room pool
    /// </summary>
    Normal,

    /// <summary>
    /// The furthest unit from the start. On hold, nothing assigns this yet
    /// </summary>
    Boss
}
