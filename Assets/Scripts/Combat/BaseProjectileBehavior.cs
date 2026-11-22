using UnityEngine;

public enum ProjectileType
{
    Player,
    Enemy
}

public class BaseProjectileBehavior : MonoBehaviour
{
    [Header("Projectile Stats")]

    [SerializeField] private int _baseProjectileDamage = 1;
    [SerializeField] private float _baseProjectileSpeed = 5f;
    [SerializeField] private float _baseProjectileLifespan = 3f;
    [SerializeField] private int _baseProjectilePierce = 1;

    private float _currentProjectileDamage;
    private float _currentProjectileSpeed;
    private float _currentProjectileLifespan;
    private float _currentProjectilePierce;

    [Header("Projectile Settings")]

    [SerializeField] private ProjectileType _type;

    [SerializeField] private GameObject enemyWhoShotThisProjectile;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    [Header("Projectile Cosmetics")]

    [SerializeField] private AudioSource _projectileExplosionSFX;

    [Header("Reflected Projectile Visuals")]

    [Tooltip("Optional. If both of these are assigned, the Resources paths below are never used.")]
    [SerializeField] private Mesh _reflectedMesh;
    [SerializeField] private Material _reflectedMaterial;

    [Tooltip("Path under a folder named Resources, with no file extension. " +
             "Used only for whichever of the two fields above is left empty. " +
             "The mesh path may point at a model (.fbx) and the mesh inside it will be found.")]
    [SerializeField] private string _reflectedMeshResourcePath = "ProjectileMeshes/ST_Baseball";
    [SerializeField] private string _reflectedMaterialResourcePath = "ProjectileMaterials/ReflectedProjectile";

    // ------------------------------------------ Internal Utilities ------------------------------------------ //

    private float _timer;
    private int _currentRemainingPierce;

    private Mesh _originalMesh;
    private Material _originalMaterial;

    private Mesh _resolvedReflectedMesh;
    private Material _resolvedReflectedMaterial;

    private ProjectileType _initialType;

    private static ThirdPersonController _playerController;
    private static StatHandler _statHandler;

    // ----------------------------------------- Unity Defaults -------------------------------------------- //

    /// <summary>
    /// A method that runs once when this GameObject is created
    /// </summary>
    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        _originalMesh = meshFilter.sharedMesh;
        _originalMaterial = meshRenderer.sharedMaterial;

        _initialType = _type;

        ResolveReflectedVisuals();
    }

    /// <summary>
    /// A method that runs once per frame
    /// </summary>
    void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            PlayExplosionSFX();
            DisableProjectile();
        }
    }

    /// <summary>
    /// A method that runs when this GameObject is enabled
    /// </summary>
    private void OnEnable()
    {
        // Pooled projectiles keep their old trail points; clear them so the trail doesn't streak from the last impact.
        if (TryGetComponent(out TrailRenderer trail))
        {
            trail.Clear();
        }

        // Undo anything a reflect changed on this instance before it was pooled.
        _type = _initialType;

        ResetProjectileMeshAndMaterial();
        SetProjectileStats();

        _timer = _currentProjectileLifespan;

        // Use the stat-modified pierce, not the base value, or pierce upgrades do nothing.
        _currentRemainingPierce = Mathf.Max(1, Mathf.RoundToInt(_currentProjectilePierce));

        Move();
    }

    /// <summary>
    /// A method that runs when this GameObject is disabled, which for a pooled projectile is the
    /// moment it is returned to the pool.
    /// </summary>
    private void OnDisable()
    {
        // Don't carry a stale shooter reference into the next use of this object.
        enemyWhoShotThisProjectile = null;
    }

    // --------------------------------------- Projectile Motion ------------------------------------------------//

    /// <Summary>
    /// Zeros the projectile's linear and angular velocity, then launches it forward with the current projectile speed
    /// </Summary>
    private void Move()
    {
        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(transform.forward * _currentProjectileSpeed, ForceMode.VelocityChange);
        }
    }

    // --------------------------------------- Collision Behavior -----------------------------------------------//

    /// <Summary>
    /// Runs when the projectile is triggered by another collider. Checks if the object collided with is an enemy or player, and causes them to take damage
    /// depending on if the projectile came from the other. Reduces projectile's remaining pierce and disables it if pierce reaches 0.
    /// </Summary>
    private void OnTriggerEnter(Collider otherCollider)
    {
        GameObject objectHit = otherCollider.gameObject;

        // Ignore our own team: player shots pass through the player,
        // enemy shots pass through enemies. Nothing is used up.
        if ((_type == ProjectileType.Player && objectHit.CompareTag("Player")) ||
            (_type == ProjectileType.Enemy && objectHit.CompareTag("Enemy")))
        {
            return;
        }

        // Hit something solid with no health (wall, floor, prop): stop here.
        if (!otherCollider.isTrigger &&
            !objectHit.TryGetComponent<HealthComponent>(out _))
        {
            DisableProjectile();
            return;
        }

        if (objectHit.TryGetComponent<HealthComponent>(out HealthComponent hc))
        {
            if (hc.gameObject.CompareTag("Player") && PlayerIsParrying())
            {
                ReflectProjectile();
                return;
            }

            // Same-team hits were filtered out above, so anything left is a valid target.
            hc.Damage((int)_currentProjectileDamage);

            _currentRemainingPierce -= 1;

            if (_currentRemainingPierce <= 0)
            {
                DisableProjectile();
            }
        }
    }

    /// <Summary>
    /// Turns an enemy projectile into a player projectile aimed back at whoever fired it, swaps in the
    /// reflected visuals, and doubles its damage.
    /// </Summary>
    private void ReflectProjectile()
    {
        _type = ProjectileType.Player;

        if (enemyWhoShotThisProjectile != null)
        {
            transform.LookAt(enemyWhoShotThisProjectile.transform.position);
        }

        SwitchProjectileMeshAndMaterial(_resolvedReflectedMesh, _resolvedReflectedMaterial);

        _currentProjectileDamage *= 2;

        Move();
    }

    /// <Summary>
    /// True if the player is currently parrying. Resolves and caches the controller on first use.
    /// </Summary>
    private bool PlayerIsParrying()
    {
        if (_playerController == null)
        {
            _playerController = FindAnyObjectByType<ThirdPersonController>();
        }

        return _playerController != null && _playerController.ParryActive();
    }

    /// <summary>
    ///  This method calls the SetActive() method to disable the object from the scene.
    /// </summary>
    private void DisableProjectile()
    {
        if (gameObject.activeSelf)
        {
            ObjectPool.instance.ReturnPooledObject(gameObject);
        }
    }

    /// <Summary>
    /// Plays the fizzle/impact sound, if one is assigned.
    /// </Summary>
    private void PlayExplosionSFX()
    {
        if (_projectileExplosionSFX == null)
        {
            return;
        }

        _projectileExplosionSFX.Play();
    }

    // ------------------------------------- Switching Projectile Mesh/Material -------------------------------------------//

    /// <summary>
    /// Switches the projectile's mesh and material to the given ones, if they are not null. If either is null, that property is left unchanged.
    /// </summary>
    /// <param name="newMesh"></param> the new mesh to use for the projectile. If null, the projectile's mesh is not changed.
    /// <param name="newMaterial"></param> the new material to use for the projectile. If null, the projectile's material is not changed.
    public void SwitchProjectileMeshAndMaterial(Mesh newMesh, Material newMaterial)
    {
        if (meshFilter == null || meshRenderer == null)
        {
            return;
        }

        if (newMesh != null)
        {
            meshFilter.sharedMesh = newMesh;
        }

        if (newMaterial != null)
        {
            meshRenderer.sharedMaterial = newMaterial;
        }
    }

    /// <summary>
    /// Resets the projectile's mesh and material to the original ones it had when it was first created. This is useful for pooled projectiles that may have been reflected and had their visuals changed.
    /// </summary>
    private void ResetProjectileMeshAndMaterial()
    {
        if (meshFilter == null || meshRenderer == null)
        {
            return;
        }

        meshFilter.sharedMesh = _originalMesh;
        meshRenderer.sharedMaterial = _originalMaterial;
    }

    /// <Summary>
    /// Works out the mesh and material used for reflected projectiles, once, at Awake.
    /// </Summary>
    private void ResolveReflectedVisuals()
    {
        _resolvedReflectedMesh = _reflectedMesh;
        _resolvedReflectedMaterial = _reflectedMaterial;

        if (_resolvedReflectedMesh == null && !string.IsNullOrEmpty(_reflectedMeshResourcePath))
        {
            _resolvedReflectedMesh = LoadMeshFromModel(_reflectedMeshResourcePath);
        }

        if (_resolvedReflectedMaterial == null && !string.IsNullOrEmpty(_reflectedMaterialResourcePath))
        {
            _resolvedReflectedMaterial = Resources.Load<Material>(_reflectedMaterialResourcePath);
        }

        if (_resolvedReflectedMesh == null && _resolvedReflectedMaterial == null)
        {
            Debug.LogWarning(
                $"{name}: reflected visuals unresolved, parried shots will keep their original look. " +
                $"mesh={(_resolvedReflectedMesh == null ? "null" : _resolvedReflectedMesh.name)} " +
                $"(path '{_reflectedMeshResourcePath}'), " +
                $"material={(_resolvedReflectedMaterial == null ? "null" : _resolvedReflectedMaterial.name)} " +
                $"(path '{_reflectedMaterialResourcePath}').",
                this);
        }
    }

    /// <Summary>
    /// Loads a model asset (an .fbx, for instance) from Resources and returns the mesh its own MeshFilter uses.
    /// </Summary>
    private static Mesh LoadMeshFromModel(string resourcePath)
    {
        GameObject model = Resources.Load<GameObject>(resourcePath);

        if (model == null)
        {
            return null;
        }

        MeshFilter modelFilter = model.GetComponentInChildren<MeshFilter>();

        return modelFilter != null ? modelFilter.sharedMesh : null;
    }

    // --------------------------------------------- Projectile Stats -------------------------------------------------- //

    /// <Summary>
    /// Sets the projetile's stats if it's a player projectile based on the stat handler's current stat values. Has protections for if the stat doesn't exist in
    /// the Stat enum by setting the current value to the base value.
    /// </Summary>
    private void SetProjectileStats()
    {
        _currentProjectileDamage = _baseProjectileDamage;
        _currentProjectileSpeed = _baseProjectileSpeed;
        _currentProjectileLifespan = _baseProjectileLifespan;
        _currentProjectilePierce = _baseProjectilePierce;

        if (_type == ProjectileType.Enemy)
        {
            return;
        }

        if (_statHandler == null)
        {
            _statHandler = FindAnyObjectByType<StatHandler>();
        }

        if (_statHandler == null)
        {
            return;
        }

        _currentProjectileDamage = (_statHandler.GetStat(Stat.AttackDamage) != 0f) ? _statHandler.GetStat(Stat.AttackDamage) * _baseProjectileDamage : _baseProjectileDamage;
        _currentProjectileSpeed = (_statHandler.GetStat(Stat.ProjectileSpeed) != 0f) ? _statHandler.GetStat(Stat.ProjectileSpeed) * _baseProjectileSpeed : _baseProjectileSpeed;
        _currentProjectileLifespan = (_statHandler.GetStat(Stat.ProjectileLifespan) != 0f) ? _statHandler.GetStat(Stat.ProjectileLifespan) * _baseProjectileLifespan : _baseProjectileLifespan;
        _currentProjectilePierce = (_statHandler.GetStat(Stat.ProjectilePierce) != 0f) ? _statHandler.GetStat(Stat.ProjectilePierce) * _baseProjectilePierce : _baseProjectilePierce;
    }

    /// <Summary>
    /// Sets only the currentProjectileDamage to a specific value
    /// </Summary>
    public void SetOnlyDamage(int damage)
    {
        _currentProjectileDamage = damage;
    }

    /// <Summary>
    /// Sets only the currentProjectileSpeed to a specific value
    /// </Summary>
    public void SetOnlySpeed(float speed)
    {
        _currentProjectileSpeed = speed;
    }

    /// <Summary>
    /// Sets only the currentProjectileLifespan to a specific value
    /// </Summary>
    public void SetOnlyLifespan(float lifespan)
    {
        _currentProjectileLifespan = lifespan;
    }

    /// <Summary>
    /// Sets only the currentProjectilePierce to a specific value
    /// </Summary>
    public void SetOnlyPierce(int pierce)
    {
        _currentProjectilePierce = pierce;
    }

    // ------------------------------------------------ Generic Setters -------------------------------------------------- //

    /// <summary>
    /// Sets the enemy who shot this projectile, so that it can be reflected back at them if the player parries it.
    /// </summary>
    /// <param name="enemy"></param> The GameObject of the enemy who shot this projectile. This is used to determine where to aim the projectile when it is reflected by the player.
    public void SetEnemyWhoShotThisProjectile(GameObject enemy)
    {
        enemyWhoShotThisProjectile = enemy;
    }
}