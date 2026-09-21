using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5.5f;
    [SerializeField] private float sprintSpeed = 9.5f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 22f;
    [SerializeField] private float rotationSpeed = 14f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -30f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController controller;

    private Vector3 velocity;

    private float currentSpeed;

    private ArjunaAim arjunaAim;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        currentSpeed = moveSpeed;

        // Find ArjunaAim anywhere inside PlayerChariot.
        arjunaAim =
            GetComponentInChildren<ArjunaAim>();

        if (arjunaAim == null)
        {
            Debug.LogWarning(
                "PlayerMovement: Could not find ArjunaAim inside PlayerChariot."
            );
        }
    }

    private void Update()
    {
        HandleMovement();

        HandleGravity();
    }

    // --------------------------------
    // MOVEMENT
    // --------------------------------

    private void HandleMovement()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            input.x =
                Keyboard.current.dKey.isPressed ? 1f : 0f;

            input.x -=
                Keyboard.current.aKey.isPressed ? 1f : 0f;

            input.y =
                Keyboard.current.wKey.isPressed ? 1f : 0f;

            input.y -=
                Keyboard.current.sKey.isPressed ? 1f : 0f;
        }

        // Prevent diagonal movement from being faster.
        input = Vector2.ClampMagnitude(input, 1f);

        if (cameraTransform == null)
        {
            Debug.LogWarning(
                "PlayerMovement: Camera Transform is missing."
            );

            return;
        }

        // --------------------------------
        // AIMING
        // --------------------------------

        bool isAiming =
            arjunaAim != null &&
            arjunaAim.IsAiming;

        // --------------------------------
        // MOVEMENT DIRECTION
        // --------------------------------

        Vector3 movement = Vector3.zero;

        // --------------------------------
        // FORWARD (W)
        // --------------------------------

        if (input.y > 0.01f)
        {
            Vector3 cameraForward =
                cameraTransform.forward;

            cameraForward.y = 0f;

            cameraForward.Normalize();

            movement = cameraForward;
        }

        // --------------------------------
        // REVERSE (S)
        // --------------------------------

        else if (input.y < -0.01f)
        {
            // IMPORTANT:
            // S moves opposite to the chariot's
            // CURRENT forward direction.

            movement =
                -transform.forward;

            movement.y = 0f;

            movement.Normalize();
        }

        // --------------------------------
        // LEFT / RIGHT
        // --------------------------------

        else if (Mathf.Abs(input.x) > 0.01f)
        {
            Vector3 cameraRight =
                cameraTransform.right;

            cameraRight.y = 0f;

            cameraRight.Normalize();

            movement = cameraRight * input.x;

            movement =
                Vector3.ClampMagnitude(
                    movement,
                    1f
                );
        }

        // --------------------------------
        // SPRINT
        // --------------------------------

        bool sprintPressed =
            Keyboard.current != null &&
            Keyboard.current.leftShiftKey.isPressed;

        bool isSprinting =
            sprintPressed &&
            !isAiming &&
            input.sqrMagnitude > 0.01f &&
            input.y > 0f;

        float targetSpeed =
            isSprinting
                ? sprintSpeed
                : moveSpeed;

        // --------------------------------
        // ACCELERATION
        // --------------------------------

        float speedChangeRate;

        if (movement.sqrMagnitude > 0.01f)
        {
            speedChangeRate =
                isSprinting
                    ? acceleration * 1.35f
                    : acceleration;
        }
        else
        {
            speedChangeRate =
                deceleration;
        }

        currentSpeed =
            Mathf.MoveTowards(
                currentSpeed,
                targetSpeed,
                speedChangeRate *
                Time.deltaTime
            );

        // --------------------------------
        // MOVE CHARIOT
        // --------------------------------

        controller.Move(
            movement *
            currentSpeed *
            Time.deltaTime
        );

        // --------------------------------
        // ROTATION
        // --------------------------------

        // IMPORTANT:
        // Only rotate when moving FORWARD.
        //
        // S will NEVER rotate the chariot.
        if (!isAiming &&
            input.y > 0.01f &&
            movement.sqrMagnitude > 0.01f)
        {
            RotateTowardsMovement(
                movement
            );
        }
    }

    // --------------------------------
    // GRAVITY
    // --------------------------------

    private void HandleGravity()
    {
        if (controller.isGrounded &&
            velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        velocity.y +=
            gravity *
            Time.deltaTime;

        controller.Move(
            velocity *
            Time.deltaTime
        );
    }

    // --------------------------------
    // ROTATION
    // --------------------------------

    private void RotateTowardsMovement(
        Vector3 movement)
    {
        Quaternion targetRotation =
            Quaternion.LookRotation(
                movement
            );

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed *
                Time.deltaTime
            );
    }
}