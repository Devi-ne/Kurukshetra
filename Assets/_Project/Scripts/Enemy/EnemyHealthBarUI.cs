using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private Image healthBarFill;

    private void Start()
    {
        if (enemyHealth == null)
        {
            enemyHealth =
                GetComponentInParent<EnemyHealth>();
        }

        UpdateHealthBar();
    }

    private void Update()
    {
        if (enemyHealth == null)
            return;

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill == null)
            return;

        if (enemyHealth == null)
            return;

        float healthPercent =
            enemyHealth.CurrentHealth /
            enemyHealth.MaxHealth;

        healthPercent =
            Mathf.Clamp01(healthPercent);

        healthBarFill.fillAmount =
            healthPercent;
    }
}