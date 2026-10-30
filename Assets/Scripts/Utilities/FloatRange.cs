using UnityEngine;
using System;

/// <Summary>
/// Serializable type for a range of floats, with fields for a min and a max
/// </Summary>
[Serializable]
public struct FloatRange
{
    public float min;
    public float max;

    public FloatRange(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}
