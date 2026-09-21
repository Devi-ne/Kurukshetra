using UnityEngine;

public class BhishmaArrow : MonoBehaviour
{
    [Header("Arrow")]
    [SerializeField] private float speed = 35f;
    [SerializeField] private float lifetime = 5f;

    [Header("Damage")]
    [SerializeField] private float damage = 25f;

    [Header("Collision")]
    [SerializeField] private float spawnProtectionTime = 0.1f;

    private Vector3 direction;
    private GameObject shooter;
    private float spawnTimer;
    private bool hasHit;

    public void Initialize(
        Vector3 shootDirection,
        float newDamage,
        GameObject newShooter
    )
    {
        direction = shootDirection.normalized;
        damage = newDamage;
        shooter = newShooter;

        if (direction.sqrMagnitude > 0.01f)
        {
            transform.rotation =
                Quaternion.LookRotation(direction);
        }

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (hasHit)
            return;

        spawnTimer += Time.deltaTime;

        transform.position +=
            direction *
            speed *
            Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit)
            return;

        if (spawnTimer < spawnProtectionTime)
            return;

        // Don't damage Bhishma
        if (shooter != null &&
            other.transform.root.gameObject == shooter)
        {
            return;
        }

        // ========================================
        // ARJUNA CHARIOT HIT
        // ========================================

        PlayerChariotHitbox chariot =
            other.GetComponentInParent<PlayerChariotHitbox>();

        if (chariot != null)
        {
            hasHit = true;

            chariot.TakeDamage(damage);

            Debug.Log(
                "Bhishma arrow hit Arjuna's chariot!"
            );

            Destroy(gameObject);

            return;
        }

        // ========================================
        // DIRECT ARJUNA HIT
        // ========================================

        IDamageable damageable =
            other.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            hasHit = true;

            damageable.TakeDamage(damage);

            Debug.Log(
                "Bhishma arrow hit Arjuna for " +
                damage +
                " damage."
            );

            Destroy(gameObject);

            return;
        }

        // ========================================
        // ENVIRONMENT
        // ========================================

        hasHit = true;

        Destroy(gameObject);
    }
}