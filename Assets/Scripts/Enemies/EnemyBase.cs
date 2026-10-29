using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Basic class for enemy prefab with state machine, holding minimal basic fields.
/// Attack timing and projectile spawning are delegated to an AttackPattern asset.
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

    /// <summary>
    /// How the enemy closes distance before attacking.
    /// ChasePlayer - walks straight at the player, attacks once in range.
    /// RandomRoam  - walks to a random reachable NavMesh point, then attacks from there.
    /// </summary>
    public enum ChaseMode
    {
        ChasePlayer,
        RandomRoam
    }

    [SerializeField] protected float speed = 10f;
    [SerializeField] protected int attackDamage = 10;

    [SerializeField] protected EnemyState currentState = EnemyState.Idle;
    [SerializeField] protected float detectionRange = 10f;
    [SerializeField] protected float attackRange = 5f;
    [SerializeField][Range(1f, 2f)] protected float exitChaseRangeMultiplier = 1.1f;
    [SerializeField][Range(1f, 2f)] protected float exitAttackRangeMultiplier = 1.1f;
    [SerializeField] protected float pathUpdateInterval = 0.2f;
    protected float pathUpdateTimer = 0f;

    [SerializeField] protected Transform playerLocation;
    [SerializeField] protected float distanceFromPlayer;

    [Header("Chase Behaviour")]
    [SerializeField] protected ChaseMode chaseMode = ChaseMode.ChasePlayer;
    [Tooltip("RandomRoam only: how far from the enemy to look for a reposition point.")]
    [SerializeField] protected float roamRadius = 12f;
    [Tooltip("RandomRoam only: give up and repick if the enemy cannot reach its point in this long.")]
    [SerializeField] protected float roamTimeout = 6f;
    protected Vector3 roamDestination;
    protected bool hasRoamDestination;
    protected float roamTimer;

    [Header("Core Attack Pattern")]
    [SerializeField] protected AttackPattern coreAttackPattern;
    [Tooltip("Where projectiles spawn from. Falls back to this transform if empty.")]
    [SerializeField] protected Transform firePoint;
    [Tooltip("Scales the pattern's windup for this enemy class. 1 is the pattern's full windup.")]
    [SerializeField][Range(0f, 2f)] protected float windupMultiplier = 1f;
    [Tooltip("Pause between finishing one attack cycle and re-evaluating.")]
    [SerializeField] protected float attackRepeatDelay = 0.5f;
    [Tooltip("How fast the enemy body turns to face its aim direction while attacking.")]
    [SerializeField] protected float turnSpeed = 540f;

    protected AttackPatternRuntime attackRuntime;
    protected float attackRepeatTimer;
    protected bool attackInProgress;

    protected NavMeshAgent agent;

    /// <summary>
    /// Update enemy state machine
    /// </summary>
    protected void SetState(EnemyState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        EnemyState previousState = currentState;
        currentState = newState;
        OnStateChanged(previousState, newState);
    }

    /// <summary>
    /// Hook for reacting to state transitions. Override to add animation triggers etc.
    /// </summary>
    protected virtual void OnStateChanged(EnemyState previousState, EnemyState newState)
    {
        switch (newState)
        {
            case EnemyState.Chase:
                pathUpdateTimer = pathUpdateInterval;
                hasRoamDestination = false;
                break;

            case EnemyState.Attack:
                BeginAttack();
                break;
        }
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

        attackRuntime = new AttackPatternRuntime();
        attackRuntime.SetContext(transform, firePoint, playerLocation);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (playerLocation == null)
            return;
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
            SetState(EnemyState.Chase);
        }
    }

    /// <summary>
    /// Moves the enemy into attack position, either by advancing on the player or by
    /// repositioning to a random point on the NavMesh, depending on chaseMode.
    /// </summary>
    protected virtual void HandleChase()
    {
        agent.isStopped = false;

        // Lost the player entirely, go back to idle in either mode.
        if (distanceFromPlayer > detectionRange * exitChaseRangeMultiplier)
        {
            SetState(EnemyState.Idle);
            return;
        }

        if (chaseMode == ChaseMode.RandomRoam)
        {
            HandleRoam();
            return;
        }

        pathUpdateTimer += Time.deltaTime;
        if (pathUpdateTimer >= pathUpdateInterval)
        {
            agent.SetDestination(playerLocation.position);
            pathUpdateTimer = 0f;
        }

        // Player is close enough to attack, go to attack state
        if (distanceFromPlayer <= attackRange)
        {
            SetState(EnemyState.Attack);
        }
    }

    /// <summary>
    /// Picks a random reachable point, walks to it, and attacks on arrival rather than
    /// closing on the player directly.
    /// </summary>
    protected virtual void HandleRoam()
    {
        if (!hasRoamDestination)
        {
            if (TryGetRandomNavMeshPoint(transform.position, roamRadius, out roamDestination))
            {
                agent.SetDestination(roamDestination);
                hasRoamDestination = true;
                roamTimer = 0f;
            }
            else
            {
                // No valid point this frame, fall back to advancing on the player.
                agent.SetDestination(playerLocation.position);
                return;
            }
        }

        roamTimer += Time.deltaTime;

        bool arrived = !agent.pathPending
                       && agent.remainingDistance <= agent.stoppingDistance + 0.25f;

        if (arrived)
        {
            hasRoamDestination = false;
            SetState(EnemyState.Attack);
        }
        else if (roamTimer >= roamTimeout)
        {
            // Stuck or the path got invalidated, pick somewhere else.
            hasRoamDestination = false;
        }
    }

    /// <summary>
    /// Samples the NavMesh for a reachable point within radius of center.
    /// </summary>
    protected bool TryGetRandomNavMeshPoint(Vector3 center, float radius, out Vector3 result)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 candidate = center + Random.insideUnitSphere * radius;

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    result = hit.position;
                    return true;
                }
            }
        }

        result = center;
        return false;
    }

    /// <summary>
    /// Starts one cycle of the attack pattern.
    /// </summary>
    protected virtual void BeginAttack()
    {
        if (coreAttackPattern == null || attackRuntime == null)
        {
            attackInProgress = false;
            return;
        }

        attackRuntime.Target = playerLocation;
        coreAttackPattern.Begin(attackRuntime, windupMultiplier);
        attackInProgress = true;
        attackRepeatTimer = 0f;
    }

    /// <summary>
    /// Runs the attack pattern to completion, then decides what to do next.
    /// The state is deliberately not re-evaluated mid-pattern so a volley is never cut
    /// off halfway through; range checks happen between cycles instead.
    /// </summary>
    protected virtual void HandleAttack()
    {
        agent.isStopped = true;

        if (coreAttackPattern == null || attackRuntime == null)
        {
            SetState(EnemyState.Chase);
            return;
        }

        FaceAimDirection();

        if (attackInProgress)
        {
            attackInProgress = coreAttackPattern.Tick(attackRuntime, Time.deltaTime);
            return;
        }

        // Pattern finished, brief breather before committing to the next action.
        attackRepeatTimer += Time.deltaTime;
        if (attackRepeatTimer < attackRepeatDelay)
        {
            return;
        }

        if (distanceFromPlayer > detectionRange * exitChaseRangeMultiplier)
        {
            SetState(EnemyState.Idle);
        }
        else if (chaseMode == ChaseMode.RandomRoam)
        {
            // Always reposition between volleys.
            SetState(EnemyState.Chase);
        }
        else if (distanceFromPlayer > attackRange * exitAttackRangeMultiplier)
        {
            SetState(EnemyState.Chase);
        }
        else
        {
            BeginAttack();
        }
    }

    /// <summary>
    /// Turns the enemy body toward whatever the pattern is currently aiming at, so the
    /// visual facing matches the direction projectiles will leave in.
    /// </summary>
    protected virtual void FaceAimDirection()
    {
        Vector3 aim = AttackPatternRuntime.Flatten(attackRuntime.AimDirection);

        if (aim.sqrMagnitude < 0.0001f)
        {
            return;
        }

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.LookRotation(aim, Vector3.up),
            turnSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Handles enemy Dead state
    /// </summary>
    protected virtual void HandleDead()
    {
        agent.isStopped = true;
        attackInProgress = false;
        //TODO: add death animation, remove from scene, etc.
    }

    public int GetAttackDamage()
    {
        return attackDamage;
    }
}