using UnityEngine;

/// <summary>
/// Basic class for enemy prefab with state machine, holding minimal basic fields
/// </summary>
public abstract class EnemyBase : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Attack,
        Dead
    }

    public int speed;
    public int attackDamage;

    public EnemyState currentState = EnemyState.Idle;

    /// <summary>
    /// Update currentState
    /// </summary>
    void setState(EnemyState newState)
    {
        currentState = newState;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
