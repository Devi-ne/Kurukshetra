using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image fillImage;

    [Header("Smooth Animation")]
    [SerializeField] private float smoothSpeed = 8f;

    private float currentFill;
    private float targetFill;

    private void Start()
    {
        if (playerHealth == null || fillImage == null)
            return;

        targetFill = GetHealthPercent();
        currentFill = targetFill;

        fillImage.fillAmount = currentFill;
    }

    private void Update()
    {
        if (playerHealth == null || fillImage == null)
            return;

        targetFill = GetHealthPercent();

        // Smooth health bar animation.
        // unscaledDeltaTime keeps it working even when Time.timeScale = 0.
        currentFill = Mathf.Lerp(
            currentFill,
            targetFill,
            smoothSpeed * Time.unscaledDeltaTime
        );

        // Force the bar to exactly reach the target
        // when it gets extremely close.
        if (Mathf.Abs(currentFill - targetFill) < 0.001f)
        {
            currentFill = targetFill;
        }

        fillImage.fillAmount = currentFill;
    }

    private float GetHealthPercent()
    {
        if (playerHealth == null || playerHealth.MaxHealth <= 0f)
            return 0f;

        return Mathf.Clamp01(
            playerHealth.CurrentHealth / playerHealth.MaxHealth
        );
    }
}