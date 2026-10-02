using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField][Range(1f, 2f)] protected float exitChaseRangeMultiplier = 1.1f;
    [SerializeField][Range(1f, 2f)] protected float exitAttackRangeMultiplier = 1.1f;
    [SerializeField] protected float pathUpdateInterval = 0.2f;
    protected float pathUpdateTimer = 0f;

    [SerializeField] protected Transform playerLocation;
    [SerializeField] protected float distanceFromPlayer;

    [SerializeField] protected float attackWindupTime = 0.5f;
    [SerializeField] protected float attackCooldownTime = 1f;

    protected NavMeshAgent agent;

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
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;

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
        agent.isStopped = true;
        if (distanceFromPlayer <= detectionRange)
        {
            pathUpdateTimer = pathUpdateInterval;
            SetState(EnemyState.Chase);
        }
    }

    /// <summary>
    /// Makes enemy actively advance towards the player
    /// </summary>
    protected virtual void HandleChase()
    {
        pathUpdateTimer += Time.deltaTime;
        agent.isStopped = false;
        if (pathUpdateTimer >= pathUpdateInterval)
        {
            agent.SetDestination(playerLocation.position);
            pathUpdateTimer = 0f;
        }
        Debug.Log("In chase mode, distance from player: " + distanceFromPlayer);

        //player is too far away, go back to idle
        if (distanceFromPlayer > detectionRange * exitChaseRangeMultiplier)
        {
            Debug.Log("entering idle mode");
            SetState(EnemyState.Idle);
        }
        //player is close enough to attack, go to attack state
        else if (distanceFromPlayer <= attackRange)
        {
            Debug.Log("entering attacks mode");
            SetState(EnemyState.Attack);
        }
    }

    protected float attackTimer = 0f;
    protected bool hasHitThisAttack = false;

    /// <summary>
    /// Makes enemy attack player
    /// </summary>
    protected virtual void HandleAttack()
    {
        if (distanceFromPlayer > attackRange)
        {
            SetState(EnemyState.Chase);
        }

        
        agent.isStopped = true;
        attackTimer += Time.deltaTime;
        Debug.Log("In attack mode, distance from player: " + distanceFromPlayer + ", attackTimer: " + attackTimer);

        // still winding up, do nothing yet (or flash a warning color here)
        if (attackTimer < attackWindupTime)
        {
            Debug.Log("Winding up attack, attackTimer: " + attackTimer);
            return;
        }

        // just crossed into "hit" territory, and haven't hit yet this cycle
        if (!hasHitThisAttack)
        {
            Debug.Log("Hitting player with attack, attackTimer: " + attackTimer);
            hasHitThisAttack = true;
        }

        // fully done with windup + cooldown, reset for next time
        if (attackTimer >= attackWindupTime + attackCooldownTime)
        {
            Debug.Log("Resetting attack cycle, attackTimer: " + attackTimer);
            attackTimer = 0f;
            hasHitThisAttack = false;
        }

<<<<<<< HEAD
        if (distanceFromPlayer > attackRange * exitAttackRangeMultiplier)
        {
            pathUpdateTimer = pathUpdateInterval;
            SetState(EnemyState.Chase);
        }
=======
        
>>>>>>> 6bcd9e65328baf7fc06569824b8a2d4b77e6c7d8
    }


    /// <summary>
    /// Handles enemy Dead state
    /// </summary>
    protected virtual void HandleDead()
    {
        Debug.Log("Enemy is dead, stopping all actions");
        agent.isStopped = true;
        //TODO: add death animation, remove from scene, etc.
    }
}
