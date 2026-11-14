using System;
using UnityEngine;

/// <summary>
/// Designer-tunable settings for generation: how many units to place and the cell size
/// </summary>
[Serializable]
public class LayoutParams
{
    /// <summary>
    /// How many units to place, counting the start and boss
    /// </summary>
    [Min(1)] public int roomCount = 5;

    /// <summary>
    /// The size of one cell in world units. The generator doesn't read this, it is applied when
    /// the layout is placed in the scene. A unit is 5x5 cells
    /// </summary>
    [Min(0.01f)] public float cellSize = 10f;
}
