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
        Chase,
        Attack,
        Dead
    }

    [SerializeField] protected float speed = 10f;
    [SerializeField] protected float attackDamage = 10f;

    [SerializeField] protected EnemyState currentState = EnemyState.Idle;
    [SerializeField] protected float detectionRange = 10f;
    [SerializeField] protected float attackRange = 5f;

    [SerializeField] protected Transform playerLocation;
    [SerializeField] protected float distanceFromPlayer;

    // ** TODO ** Navmesh setup

    /// <summary>
    /// Update enemy state machine
    /// </summary>
    protected void SetState(EnemyState newState)
    {
        currentState = newState;
    }

    /// <summary>
    /// returns the current enemy state
    /// </summary>
    public EnemyState GetState()
    {
        return currentState;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        playerLocation = GameObject.FindGameObjectWithTag("Player")?.transform;
        // ** TODO ** Navmesh setup
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (playerLocation == null) return;
        distanceFromPlayer = Vector3.Distance(transform.position, playerLocation.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdle();
                break;

            case EnemyState.Chase:
                HandleChase();
                break;

            case EnemyState.Attack:
                HandleAttack();
                break;

            case EnemyState.Dead:
                HandleDead();
                break;
        }
    }

    /// <summary>
    /// Handles enemy Idle state
    /// </summary>
    protected virtual void HandleIdle()
    {
        if (distanceFromPlayer <= detectionRange)
        {
            SetState(EnemyState.Chase);
        }
    }

    /// <summary>
    /// Makes enemy actively advance towards the player
    /// </summary>
    protected virtual void HandleChase()
    {
        // ** TODO ** Navmesh for: enemy advancing towards player
        if (distanceFromPlayer > detectionRange)
        {
            SetState(EnemyState.Idle);
        }
        else if (distanceFromPlayer <= attackRange)
        {
            SetState(EnemyState.Attack);
        }
    }

    /// <summary>
    /// Makes enemy attack player
    /// </summary>
    protected virtual void HandleAttack()
    {
        if (distanceFromPlayer > attackRange)
        {
            SetState(EnemyState.Chase);
        }
    }
    /// <summary>
    /// Handles enemy Dead state
    /// </summary>
    protected virtual void HandleDead()
    {

    }
}
