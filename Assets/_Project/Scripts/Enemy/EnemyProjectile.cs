using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private float damage = 8f;
    [SerializeField] private float lifetime = 5f;

    private Vector3 direction;
    private GameObject owner;

    public void Initialize(
        Transform target,
        float newDamage,
        GameObject newOwner
    )
    {
        damage = newDamage;
        owner = newOwner;

        // Calculate the direction ONLY ONCE when the arrow is fired
        direction =
            (target.position - transform.position).normalized;

        // Point the arrow toward its initial target
        if (direction.sqrMagnitude > 0.01f)
        {
            transform.rotation =
                Quaternion.LookRotation(direction);
        }

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move in a straight line
        transform.position +=
            direction *
            speed *
            Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Don't hit the archer who fired the arrow
        if (owner != null &&
            other.transform.root.gameObject == owner)
        {
            return;
        }

        IDamageable damageable =
            other.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);

            Debug.Log(
                "Enemy arrow hit " +
                other.gameObject.name +
                " for " +
                damage +
                " damage."
            );

            Destroy(gameObject);
        }
    }
}