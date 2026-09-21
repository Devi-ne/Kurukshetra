using UnityEngine;

public class BhishmaBoss : MonoBehaviour, IDamageable
{
    [Header("Boss Health")]
    [SerializeField] private float maxHealth = 500f;

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Bhishma")]
    [SerializeField] private Transform bhishmaTransform;
    [SerializeField] private float rotationSpeed = 8f;

    [Header("Bow")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject arrowPrefab;

    [Header("Attack")]
    [SerializeField] private float attackRange = 50f;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private float arrowDamage = 35f;

    private float currentHealth;
    private float nextAttackTime;

    private bool isDead;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    private void Start()
    {
        FindArjuna();

        if (firePoint == null)
        {
            Debug.LogError(
                "BHISHMA: Fire Point is missing!"
            );
        }

        if (arrowPrefab == null)
        {
            Debug.LogError(
                "BHISHMA: Arrow Prefab is missing!"
            );
        }
    }

    private void Update()
    {
        if (isDead)
            return;

        if (target == null)
        {
            FindArjuna();
            return;
        }

        if (!target.gameObject.activeInHierarchy)
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                target.position
            );

        if (distance > attackRange)
            return;

        FaceArjuna();

        if (Time.time >= nextAttackTime)
        {
            ShootArrow();

            nextAttackTime =
                Time.time + attackCooldown;
        }
    }

    // =========================================================
    // FIND ARJUNA
    // =========================================================

    private void FindArjuna()
    {
        GameObject arjuna =
            GameObject.FindGameObjectWithTag("Player");

        if (arjuna != null)
        {
            target = arjuna.transform;

            Debug.Log(
                "BHISHMA: Target found -> " +
                target.name
            );
        }
        else
        {
            Debug.LogWarning(
                "BHISHMA: Cannot find object tagged Player."
            );
        }
    }

    // =========================================================
    // FACE ARJUNA
    // =========================================================

    private void FaceArjuna()
    {
        if (bhishmaTransform == null)
            return;

        Vector3 direction =
            target.position -
            bhishmaTransform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion desiredRotation =
            Quaternion.LookRotation(direction);

        bhishmaTransform.rotation =
            Quaternion.Slerp(
                bhishmaTransform.rotation,
                desiredRotation,
                rotationSpeed * Time.deltaTime
            );
    }

    // =========================================================
    // SHOOT
    // =========================================================

    private void ShootArrow()
    {
        if (firePoint == null)
            return;

        if (arrowPrefab == null)
            return;

        if (target == null)
            return;

        Vector3 direction =
            target.position -
            firePoint.position;

        direction.Normalize();

        GameObject arrow =
            Instantiate(
                arrowPrefab,
                firePoint.position,
                Quaternion.LookRotation(direction)
            );

        BhishmaArrow projectile =
            arrow.GetComponent<BhishmaArrow>();

        if (projectile == null)
        {
            Debug.LogError(
                "BHISHMA: Arrow prefab does not have " +
                "BhishmaArrow.cs!"
            );

            Destroy(arrow);
            return;
        }

        projectile.Initialize(
            direction,
            arrowDamage,
            gameObject
        );

        Debug.Log(
            "🏹 BHISHMA FIRED AN ARROW!"
        );
    }

    // =========================================================
    // DAMAGE
    // =========================================================

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        if (damage <= 0f)
            return;

        currentHealth -= damage;

        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0f,
                maxHealth
            );

        Debug.Log(
            "⚔️ BHISHMA TOOK " +
            damage +
            " DAMAGE | HP: " +
            currentHealth +
            "/" +
            maxHealth
        );

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    // =========================================================
    // DEATH
    // =========================================================

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log(
            "🔥 BHISHMA PITAMAH DEFEATED!"
        );

        // Temporary.
        // Later we'll replace this with the
        // actual final-boss death sequence.
        gameObject.SetActive(false);
    }
}