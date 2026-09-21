using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("Enemy Type")]
    [SerializeField] private EnemyType enemyType = EnemyType.Sword;

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Archer")]
    [SerializeField] private float archerMinimumDistance = 7f;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;

    private float nextAttackTime;

    private void Start()
    {
        if (target == null)
        {
            GameObject arjuna =
                GameObject.FindGameObjectWithTag("Player");

            if (arjuna != null)
            {
                target = arjuna.transform;
            }
            else
            {
                Debug.LogError(
                    "EnemyCombat: No object with Player tag found."
                );
            }
        }

        SetupEnemyType();
    }

    private void Update()
    {
        if (target == null)
            return;

        if (!target.gameObject.activeInHierarchy)
            return;

        float distance = Vector3.Distance(
            transform.position,
            target.position
        );

        // Archer-specific attack behaviour
        if (enemyType == EnemyType.Archer)
        {
            HandleArcherCombat(distance);
        }
        else
        {
            // Normal melee enemies
            if (distance <= attackRange)
            {
                TryAttack();
            }
        }
    }

    private void HandleArcherCombat(float distance)
    {
        // TOO CLOSE
        // Don't shoot while trying to create distance.
        if (distance < archerMinimumDistance)
        {
            return;
        }

        // Within shooting range
        if (distance <= attackRange)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (Time.time < nextAttackTime)
            return;

        nextAttackTime =
            Time.time + attackCooldown;

        if (enemyType == EnemyType.Archer)
        {
            ShootArrow();
        }
        else
        {
            MeleeAttack();
        }
    }

    private void MeleeAttack()
    {
        if (target == null)
            return;

        IDamageable damageable =
            target.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(attackDamage);

            Debug.Log(
                enemyType +
                " enemy attacked Arjuna for " +
                attackDamage +
                " damage."
            );
        }
    }

    private void ShootArrow()
    {
        if (arrowPrefab == null)
        {
            Debug.LogWarning(
                "EnemyCombat: Arrow Prefab is missing."
            );

            return;
        }

        if (arrowSpawnPoint == null)
        {
            Debug.LogWarning(
                "EnemyCombat: Arrow Spawn Point is missing."
            );

            return;
        }

        GameObject arrow =
            Instantiate(
                arrowPrefab,
                arrowSpawnPoint.position,
                Quaternion.identity
            );

        EnemyProjectile projectile =
            arrow.GetComponent<EnemyProjectile>();

        if (projectile == null)
        {
            Debug.LogError(
                "EnemyCombat: Arrow prefab does not have " +
                "EnemyProjectile attached."
            );

            Destroy(arrow);

            return;
        }

        projectile.Initialize(
            target,
            attackDamage,
            gameObject
        );

        Debug.Log("Archer fired an arrow!");
    }

    private void SetupEnemyType()
    {
        switch (enemyType)
        {
            case EnemyType.Sword:

                attackRange = 2f;
                attackDamage = 10f;
                attackCooldown = 1.5f;

                break;

            case EnemyType.Spear:

                attackRange = 3.5f;
                attackDamage = 15f;
                attackCooldown = 1.8f;

                break;

            case EnemyType.Archer:

                attackRange = 20f;
                attackDamage = 8f;
                attackCooldown = 2f;

                break;
        }
    }
}