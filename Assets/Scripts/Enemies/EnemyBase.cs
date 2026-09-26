using UnityEngine;

/// <summary>
/// Basic class for enemy prefab with state machine, holding minimal basic fields
/// </summary>
public abstract class EnemyBase : MonoBehaviour
{
    // Basic Enemy state machine, open to additions!
    public enum EnemyState
    {
        Idle,
        Attack,
        Dead
    }

    [SerializeField] protected float speed = 10f;
    [SerializeField] protected float attackDamage = 10f;

    [SerializeField] protected EnemyState currentState = EnemyState.Idle;

    /// <summary>
    /// Update enemy state machine
    /// </summary>
    protected void SetState(EnemyState newState)
    {
        currentState = newState;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }
}
