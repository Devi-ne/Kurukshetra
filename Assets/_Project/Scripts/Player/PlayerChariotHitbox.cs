using UnityEngine;

public class PlayerChariotHitbox : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerHealth playerHealth;

    private void Awake()
    {
        if (playerHealth == null)
        {
            playerHealth =
                GetComponentInParent<PlayerHealth>();
        }

        if (playerHealth == null)
        {
            Debug.LogError(
                "PlayerChariotHitbox: Could not find PlayerHealth!"
            );
        }
    }

    public void TakeDamage(float damage)
    {
        if (playerHealth == null)
            return;

        playerHealth.TakeDamage(damage);

        Debug.Log(
            "Arjuna's chariot was hit! " +
            "Arjuna took " + damage + " damage."
        );
    }
}