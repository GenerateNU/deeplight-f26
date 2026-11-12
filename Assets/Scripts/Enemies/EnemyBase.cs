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
        RandomRoam,
        RandomOrbit
    }

    [Header("Generics")]
    [Tooltip("Currently obsolete since attack damage is projectile-dependent.")]
    [SerializeField] protected int attackDamage = 10;
    [SerializeField] protected Transform playerLocation;
    [SerializeField] protected float distanceFromPlayer;
    [SerializeField] protected EnemyState currentState = EnemyState.Idle;

    [Header("Enemy Movement Specifics")]
    [Tooltip("Enemy movement speed during chase phase.")]
    [SerializeField] protected float speed = 10f;
    [Tooltip("The range from the player at which the enemy will begin to chase.")]
    [SerializeField] protected float detectionRange = 10f;
    [Tooltip("ChasePlayer: the range at which an enemy will stop chasing the player and begin it's attack. RandomOrbit: default radius from player the enemy will orbit if roamOrbitRadius is 0.")]
    [SerializeField] protected float attackRange = 5f;
    [Tooltip("ChasePlayer only: enemy will idle when the player is detectionRange * exitChaseRangeMultiplier away.")]
    [SerializeField][Range(1f, 2f)] protected float exitChaseRangeMultiplier = 1.1f;
    [Tooltip("ChasePlayer only: enemy will chase when the player is attackRange * exitAttackRangeMultiplier away.")]
    [SerializeField][Range(1f, 2f)] protected float exitAttackRangeMultiplier = 1.1f;
    [Tooltip("The interval of time between the each time the enemy will check the player's position (longer interval => less accurate position but less taxing on hardware)")]
    [SerializeField] protected float pathUpdateInterval = 0.2f;
    protected float pathUpdateTimer = 0f;

    [Header("Chase Behaviour")]
    [Tooltip("The behavior of the enemy between attacks: ChasePlayer => chases player until in attack range; RandomRoam => randomly roams within its roam radius; RandomOrbit => moves to a random position around the player at its roam orbit radius.")]
    [SerializeField] protected ChaseMode chaseMode = ChaseMode.ChasePlayer;
    [Tooltip("RandomRoam only: how far from the enemy to look for a reposition point.")]
    [SerializeField] protected float roamRadius = 12f;
    [Tooltip("RandomOrbit only: how far from the player the enemy tries to hold. 0 uses attack range.")]
    [SerializeField] protected float roamOrbitRadius = 0f;
    [Tooltip("RandomOrbit only: how much the orbit distance varies per reposition, plus or minus.")]
    [SerializeField] protected float roamRadiusJitter = 2f;
    [Tooltip("RandomOrbit only: smallest arc around the player the enemy will move, in degrees.")]
    [SerializeField] protected float minRoamAngle = 40f;
    [Tooltip("RandomOrbit only: largest arc around the player the enemy will move, in degrees.")]
    [SerializeField] protected float maxRoamAngle = 140f;
    [Tooltip("Both roaming modes: give up and attack in place if the point is not reached in this long.")]
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

        if (chaseMode == ChaseMode.RandomRoam || chaseMode == ChaseMode.RandomOrbit)
        {
            HandleRoam();
            return;
        }

        // Lost the player entirely, go back to idle in either mode.
        if (distanceFromPlayer > detectionRange * exitChaseRangeMultiplier)
        {
            SetState(EnemyState.Idle);
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
            bool found = chaseMode == ChaseMode.RandomOrbit
                ? TryGetRoamPointNearPlayer(out roamDestination)
                : TryGetRandomNavMeshPoint(transform.position, roamRadius, out roamDestination);

            if (found)
            {
                agent.SetDestination(roamDestination);
                hasRoamDestination = true;
                roamTimer = 0f;
            }
            else
            {
                // Nowhere reachable to go, attack from where we stand.
                SetState(EnemyState.Attack);
                return;
            }
        }

        roamTimer += Time.deltaTime;

        bool arrived = roamTimer > 0.1f
                       && !agent.pathPending
                       && agent.remainingDistance <= agent.stoppingDistance + 0.25f;

        if (arrived || roamTimer >= roamTimeout)
        {
            agent.ResetPath();
            hasRoamDestination = false;
            SetState(EnemyState.Attack);
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
    /// Picks a reachable point on a ring around the player, a random arc away from where the
    /// enemy currently stands, so roamers circle the player instead of wandering the level.
    /// </summary>
    protected bool TryGetRoamPointNearPlayer(out Vector3 result)
    {
        result = transform.position;

        if (playerLocation == null)
        {
            return false;
        }

        Vector3 center = playerLocation.position;
        float radius = roamOrbitRadius > 0f ? roamOrbitRadius : attackRange;

        // Where the enemy sits on the ring right now. If it is standing on the player,
        // any starting angle will do.
        Vector3 fromPlayer = AttackPatternRuntime.Flatten(transform.position - center);
        if (fromPlayer == Vector3.zero)
        {
            fromPlayer = Vector3.forward;
        }

        float currentAngle = Mathf.Atan2(fromPlayer.x, fromPlayer.z) * Mathf.Rad2Deg;

        for (int i = 0; i < 10; i++)
        {
            // Move a meaningful arc around the ring, either way round.
            float arc = Random.Range(minRoamAngle, maxRoamAngle);
            if (Random.value < 0.5f)
            {
                arc = -arc;
            }

            float distance = Mathf.Max(1f, radius + Random.Range(-roamRadiusJitter, roamRadiusJitter));
            Vector3 direction = Quaternion.Euler(0f, currentAngle + arc, 0f) * Vector3.forward;
            Vector3 candidate = center + (direction * distance);

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

        if (chaseMode == ChaseMode.RandomRoam || chaseMode == ChaseMode.RandomOrbit)
        {
            SetState(EnemyState.Chase);
        }
        else if (distanceFromPlayer > detectionRange * exitChaseRangeMultiplier)
        {
            SetState(EnemyState.Idle);
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
