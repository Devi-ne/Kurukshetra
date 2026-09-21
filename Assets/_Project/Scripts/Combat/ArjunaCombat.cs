using UnityEngine;
using UnityEngine.InputSystem;

public class ArjunaCombat : MonoBehaviour
{
    [Header("Light Attack")]
    [SerializeField] private float lightDamage = 20f;

    [Header("Heavy Attack")]
    [SerializeField] private float heavyDamage = 60f;
    [SerializeField] private float heavyChargeTime = 1.5f;

    [Header("Attack Timing")]
    [SerializeField] private float attackCooldown = 0.25f;

    [Header("Heavy Charge VFX")]
    [SerializeField] private GameObject heavyChargeEffect;

    [Header("Arrow")]
    [SerializeField] private Transform gandivaFirePoint;
    [SerializeField] private GameObject arrowPrefab;

    [Header("Aim")]
    [SerializeField] private Transform cameraTransform;

    private float nextAttackTime;

    private bool isCharging;
    private float chargeTimer;
    private bool chargeEffectActivated;

    // ========================================
    // UPDATE
    // ========================================

    private void Update()
    {
        if (Mouse.current == null)
            return;

        HandleAttackInput();
    }

    // ========================================
    // ATTACK INPUT
    // ========================================

    private void HandleAttackInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Time.time >= nextAttackTime)
            {
                StartCharging();
            }
        }

        if (isCharging &&
            Mouse.current.leftButton.isPressed)
        {
            ChargeAttack();
        }

        if (isCharging &&
            Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ReleaseAttack();
        }
    }

    // ========================================
    // START CHARGING
    // ========================================

    private void StartCharging()
    {
        isCharging = true;

        chargeTimer = 0f;

        chargeEffectActivated = false;

        SetChargeEffect(false);

        Debug.Log("Gandiva charging...");
    }

    // ========================================
    // CHARGE
    // ========================================

    private void ChargeAttack()
    {
        chargeTimer += Time.deltaTime;

        chargeTimer =
            Mathf.Min(
                chargeTimer,
                heavyChargeTime
            );

        if (chargeTimer >= heavyChargeTime &&
            !chargeEffectActivated)
        {
            chargeEffectActivated = true;

            SetChargeEffect(true);

            Debug.Log("HEAVY ATTACK READY!");
        }
    }

    // ========================================
    // RELEASE
    // ========================================

    private void ReleaseAttack()
    {
        isCharging = false;

        SetChargeEffect(false);

        if (Time.time < nextAttackTime)
        {
            ResetCharge();
            return;
        }

        nextAttackTime =
            Time.time + attackCooldown;

        if (chargeTimer >= heavyChargeTime)
        {
            FireArrow(true);
        }
        else
        {
            FireArrow(false);
        }

        ResetCharge();
    }

    // ========================================
    // RESET
    // ========================================

    private void ResetCharge()
    {
        chargeTimer = 0f;

        chargeEffectActivated = false;

        isCharging = false;

        SetChargeEffect(false);
    }

    // ========================================
    // CHARGE VFX
    // ========================================

    private void SetChargeEffect(bool active)
    {
        if (heavyChargeEffect == null)
            return;

        heavyChargeEffect.SetActive(active);

        if (active)
        {
            ParticleSystem particles =
                heavyChargeEffect.GetComponent<ParticleSystem>();

            if (particles != null)
            {
                particles.Clear();
                particles.Play();
            }
        }
    }

    // ========================================
    // FIRE ARROW
    // ========================================

    private void FireArrow(bool heavy)
    {
        // ----------------------------------------
        // VALIDATION
        // ----------------------------------------

        if (gandivaFirePoint == null)
        {
            Debug.LogError(
                "ArjunaCombat: Gandiva Fire Point is missing!"
            );

            return;
        }

        if (arrowPrefab == null)
        {
            Debug.LogError(
                "ArjunaCombat: Arrow Prefab is missing!"
            );

            return;
        }

        if (cameraTransform == null)
        {
            Debug.LogError(
                "ArjunaCombat: Camera Transform is missing!"
            );

            return;
        }

        Camera cam =
            cameraTransform.GetComponent<Camera>();

        if (cam == null)
        {
            Debug.LogError(
                "ArjunaCombat: Camera Transform has no Camera component!"
            );

            return;
        }

        // ----------------------------------------
        // FIND ARJUNA HEALTH
        // ----------------------------------------

        PlayerHealth playerHealth =
            GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogError(
                "ArjunaCombat: PlayerHealth could not be found!"
            );

            return;
        }

        Transform shooterRoot =
            playerHealth.transform.root;

        // ----------------------------------------
        // CAMERA CENTER RAY
        // ----------------------------------------

        Vector3 screenCenter =
            new Vector3(
                Screen.width * 0.5f,
                Screen.height * 0.5f,
                0f
            );

        Ray centerRay =
            cam.ScreenPointToRay(screenCenter);

        Vector3 arrowDirection =
            centerRay.direction.normalized;

        // ----------------------------------------
        // SPAWN POSITION
        // ----------------------------------------

        Vector3 spawnPosition =
            gandivaFirePoint.position;

        // Move the arrow slightly forward from
        // the bow so it doesn't visually spawn
        // inside Gandiva.
        spawnPosition +=
            arrowDirection * 0.5f;

        // ----------------------------------------
        // CREATE ARROW
        // ----------------------------------------

        GameObject arrow =
            Instantiate(
                arrowPrefab,
                spawnPosition,
                Quaternion.LookRotation(
                    arrowDirection
                )
            );

        // Make absolutely sure the instantiated
        // arrow is active.
        arrow.SetActive(true);

        Debug.Log(
            "ARROW CREATED: " +
            arrow.name +
            " at " +
            spawnPosition
        );

        // ----------------------------------------
        // FIND PROJECTILE
        // ----------------------------------------
        //
        // Search both the root AND children.
        // This fixes prefabs where ArrowProjectile
        // is attached to a child object.
        // ----------------------------------------

        ArrowProjectile projectile =
            arrow.GetComponent<ArrowProjectile>();

        if (projectile == null)
        {
            projectile =
                arrow.GetComponentInChildren<ArrowProjectile>(
                    true
                );
        }

        if (projectile == null)
        {
            Debug.LogError(
                "ArjunaCombat: Arrow prefab contains " +
                "NO ArrowProjectile component!"
            );

            Destroy(arrow);

            return;
        }

        // ----------------------------------------
        // SET SHOOTER
        // ----------------------------------------

        projectile.SetShooter(
            shooterRoot
        );

        // ----------------------------------------
        // SET ATTACK TYPE
        // ----------------------------------------

        if (heavy)
        {
            projectile.SetHeavyAttack(
                heavyDamage
            );

            Debug.Log(
                "HEAVY ARROW FIRED!"
            );
        }
        else
        {
            projectile.SetLightAttack(
                lightDamage
            );

            Debug.Log(
                "LIGHT ARROW FIRED!"
            );
        }

        // ----------------------------------------
        // DEBUG
        // ----------------------------------------

        Debug.DrawRay(
            spawnPosition,
            arrowDirection * 30f,
            heavy
                ? Color.red
                : Color.white,
            3f
        );
    }

    // ========================================
    // PUBLIC CHARGE INFO
    // ========================================

    public bool IsCharging
    {
        get
        {
            return isCharging;
        }
    }

    public float ChargePercent
    {
        get
        {
            if (!isCharging ||
                heavyChargeTime <= 0f)
            {
                return 0f;
            }

            return Mathf.Clamp01(
                chargeTimer /
                heavyChargeTime
            );
        }
    }

    public bool IsFullyCharged
    {
        get
        {
            return isCharging &&
                   chargeTimer >= heavyChargeTime;
        }
    }
}