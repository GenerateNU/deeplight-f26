using UnityEngine;

/// <summary>
/// TODO
/// Each enemy owns one of these. It holds the timer, so enemies don't share one since AttackPattern is a scriptable object.
/// </summary>
public class AttackPatternRuntime
{
    public AttackPhase Phase = AttackPhase.Done;
    public float Timer;
    public int ShotIndex;

    /// <summary>
    /// ...
    /// </summary>
    public enum AttackPhase
    {
        Windup,
        Firing,
        Done
    }
}
