using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [Header("Light Arrow")]
    [SerializeField] private float lightSpeed = 30f;
    [SerializeField] private float lightDamage = 20f;

    [Header("Heavy Arrow")]
    [SerializeField] private float heavySpeed = 45f;
    [SerializeField] private float heavyDefaultDamage = 60f;
    [SerializeField] private float heavyScale = 1.6f;

    [Header("General")]
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float spawnProtectionTime = 0.1f;

    [Header("Impact")]
    [SerializeField] private GameObject hitEffect;

    [Header("Collision")]
    [SerializeField] private bool destroyOnAnyCollision = true;

    // ========================================
    // INTERNAL STATE
    // ========================================

    private bool hasHit;
    private bool isHeavy;

    private float speed;
    private float damage;
    private float spawnTimer;

    private Transform shooter;
    private Collider projectileCollider;

    // ========================================
    // SHOOTER
    // ========================================

    public void SetShooter(
        Transform shooterTransform)
    {
        shooter = shooterTransform;

        projectileCollider =
            GetComponent<Collider>();

        IgnoreShooterCollisions();
    }

    // ========================================
    // IGNORE SHOOTER COLLISIONS
    // ========================================

    private void IgnoreShooterCollisions()
    {
        if (projectileCollider == null)
            return;

        if (shooter == null)
            return;

        Collider[] shooterColliders =
            shooter.GetComponentsInChildren<Collider>(
                true
            );

        foreach (Collider shooterCollider
                 in shooterColliders)
        {
            if (shooterCollider == null)
                continue;

            if (shooterCollider == projectileCollider)
                continue;

            Physics.IgnoreCollision(
                projectileCollider,
                shooterCollider,
                true
            );
        }
    }

    // ========================================
    // LIGHT ATTACK
    // ========================================

    public void SetLightAttack(
        float customDamage)
    {
        isHeavy = false;

        speed = lightSpeed;

        damage =
            customDamage > 0f
                ? customDamage
                : lightDamage;
    }

    // ========================================
    // HEAVY ATTACK
    // ========================================

    public void SetHeavyAttack(
        float customDamage)
    {
        isHeavy = true;

        speed = heavySpeed;

        damage =
            customDamage > 0f
                ? customDamage
                : heavyDefaultDamage;

        transform.localScale *= heavyScale;
    }

    // ========================================
    // START
    // ========================================

    private void Start()
    {
        if (projectileCollider == null)
        {
            projectileCollider =
                GetComponent<Collider>();
        }

        // Perform this again for safety.
        IgnoreShooterCollisions();

        if (speed <= 0f)
        {
            speed = lightSpeed;
        }

        if (damage <= 0f)
        {
            damage = lightDamage;
        }

        Destroy(
            gameObject,
            lifetime
        );
    }

    // ========================================
    // MOVEMENT
    // ========================================

    private void Update()
    {
        if (hasHit)
            return;

        spawnTimer +=
            Time.deltaTime;

        transform.position +=
            transform.forward *
            speed *
            Time.deltaTime;
    }

    // ========================================
    // IS THIS THE SHOOTER?
    // ========================================

    private bool IsShooterCollider(
        Collider other)
    {
        if (shooter == null ||
            other == null)
        {
            return false;
        }

        Transform otherTransform =
            other.transform;

        // ----------------------------------------
        // Direct shooter
        // ----------------------------------------

        if (otherTransform == shooter)
        {
            return true;
        }

        // ----------------------------------------
        // Anything underneath shooter
        // ----------------------------------------

        if (otherTransform.IsChildOf(shooter))
        {
            return true;
        }

        // ----------------------------------------
        // Same root
        // ----------------------------------------

        if (otherTransform.root == shooter.root)
        {
            return true;
        }

        // ----------------------------------------
        // PlayerHealth safety check
        // ----------------------------------------

        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
        {
            Transform playerRoot =
                playerHealth.transform.root;

            if (playerRoot == shooter.root)
            {
                return true;
            }
        }

        return false;
    }

    // ========================================
    // COLLISION
    // ========================================

    private void OnTriggerEnter(
        Collider other)
    {
        if (hasHit)
            return;

        // ========================================
        // NEVER HIT SHOOTER
        // ========================================

        if (IsShooterCollider(other))
        {
            return;
        }

        // ========================================
        // SPAWN PROTECTION
        // ========================================

        if (spawnTimer < spawnProtectionTime)
        {
            return;
        }

        // ========================================
        // FIND DAMAGEABLE
        // ========================================

        IDamageable damageable =
            other.GetComponent<IDamageable>();

        if (damageable == null)
        {
            damageable =
                other.GetComponentInParent<IDamageable>();
        }

        // ========================================
        // HIT DAMAGEABLE
        // ========================================

        if (damageable != null)
        {
            hasHit = true;

            damageable.TakeDamage(
                damage
            );

            if (isHeavy)
            {
                Debug.Log(
                    "💥 HEAVY ARROW HIT! Damage: " +
                    damage
                );
            }
            else
            {
                Debug.Log(
                    "🏹 LIGHT ARROW HIT! Damage: " +
                    damage
                );
            }

            // ========================================
            // HIT EFFECT
            // ========================================

            if (hitEffect != null)
            {
                GameObject effect =
                    Instantiate(
                        hitEffect,
                        transform.position,
                        Quaternion.LookRotation(
                            -transform.forward
                        )
                    );

                if (isHeavy)
                {
                    effect.transform.localScale *= 2f;
                }
            }

            Destroy(gameObject);

            return;
        }

        // ========================================
        // HIT ENVIRONMENT
        // ========================================

        if (destroyOnAnyCollision)
        {
            hasHit = true;

            if (isHeavy)
            {
                Debug.Log(
                    "💥 Heavy arrow hit environment."
                );
            }
            else
            {
                Debug.Log(
                    "🏹 Light arrow hit environment."
                );
            }

            Destroy(gameObject);
        }
    }
}