using System;
using UnityEngine;

/// <summary>
/// Designer-tunable settings for generation: how many units to place and the unit size
/// </summary>
[Serializable]
public class LayoutParams
{
    /// <summary>
    /// How many units to place, counting the start and boss
    /// </summary>
    [Min(1)] public int roomCount = 5;

    /// <summary>
    /// The width of one whole node (room plus corridors) in world units. The generator doesn't
    /// read this, it is applied when the layout is placed in the scene. Must match the width of
    /// the room prefabs, so it is fixed in code instead of set in the Inspector
    /// </summary>
    public const float UnitSize = 60f;

}
