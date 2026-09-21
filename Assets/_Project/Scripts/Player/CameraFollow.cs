using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Normal Camera")]
    [SerializeField] private float distance = 6f;
    [SerializeField] private float height = 2.8f;

    [Header("Aim Camera")]
    [SerializeField] private float aimDistance = 3.5f;
    [SerializeField] private float aimHeight = 2.2f;
    [SerializeField] private float aimSideOffset = 1.2f;

    [Header("Follow")]
    [SerializeField] private float positionSmoothTime = 0.08f;
    [SerializeField] private float rotationSmoothSpeed = 15f;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 150f;
    [SerializeField] private float minVerticalAngle = -30f;
    [SerializeField] private float maxVerticalAngle = 60f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float maxDistance = 10f;

    [Header("Camera Stabilization")]
    [SerializeField] private float stabilizeSmoothSpeed = 8f;

    [Header("Stabilized Camera")]
    [SerializeField] private float stabilizedDistance = 6f;
    [SerializeField] private float stabilizedHeight = 2.8f;

    private float yaw;
    private float pitch = 15f;

    private Vector3 currentVelocity;

    private ArjunaAim arjunaAim;

    private bool isStabilized;

    private void Awake()
    {
        if (target != null)
        {
            arjunaAim =
                target.GetComponentInParent<ArjunaAim>();

            if (arjunaAim == null)
            {
                arjunaAim =
                    target.GetComponentInChildren<ArjunaAim>();
            }
        }
    }

    private void Start()
    {
        if (target != null)
        {
            yaw = target.eulerAngles.y;
        }
    }

    private void LateUpdate()
    {
        if (target == null ||
            Mouse.current == null)
        {
            return;
        }

        HandleStabilizationToggle();

        if (!isStabilized)
        {
            HandleMouseLook();
            HandleZoom();
        }
        else
        {
            HandleStabilizedCamera();
        }

        HandleCameraPosition();
    }

    // --------------------------------
    // STABILIZATION TOGGLE
    // --------------------------------

    private void HandleStabilizationToggle()
    {
        if (Keyboard.current != null &&
            Keyboard.current.capsLockKey.wasPressedThisFrame)
        {
            isStabilized = !isStabilized;

            if (isStabilized)
            {
                // When stabilization starts,
                // smoothly align camera behind chariot.
                yaw =
                    target.eulerAngles.y;
            }
        }
    }

    // --------------------------------
    // MOUSE LOOK
    // --------------------------------

    private void HandleMouseLook()
    {
        Vector2 mouseDelta =
            Mouse.current.delta.ReadValue();

        yaw +=
            mouseDelta.x *
            mouseSensitivity *
            Time.deltaTime;

        pitch -=
            mouseDelta.y *
            mouseSensitivity *
            Time.deltaTime;

        pitch =
            Mathf.Clamp(
                pitch,
                minVerticalAngle,
                maxVerticalAngle
            );
    }

    // --------------------------------
    // ZOOM
    // --------------------------------

    private void HandleZoom()
    {
        float scroll =
            Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scroll) > 0.01f)
        {
            distance -=
                scroll *
                zoomSpeed *
                0.01f;

            distance =
                Mathf.Clamp(
                    distance,
                    minDistance,
                    maxDistance
                );
        }
    }

    // --------------------------------
    // STABILIZED CAMERA
    // --------------------------------

    private void HandleStabilizedCamera()
    {
        float targetYaw =
            target.eulerAngles.y;

        yaw =
            Mathf.LerpAngle(
                yaw,
                targetYaw,
                stabilizeSmoothSpeed *
                Time.deltaTime
            );

        // Keep the camera at a comfortable
        // third-person pitch.
        pitch =
            Mathf.Lerp(
                pitch,
                15f,
                stabilizeSmoothSpeed *
                Time.deltaTime
            );
    }

    // --------------------------------
    // CAMERA POSITION
    // --------------------------------

    private void HandleCameraPosition()
    {
        bool isAiming =
            arjunaAim != null &&
            arjunaAim.IsAiming;

        float currentDistance;
        float currentHeight;

        if (isStabilized)
        {
            currentDistance =
                stabilizedDistance;

            currentHeight =
                stabilizedHeight;
        }
        else
        {
            currentDistance =
                isAiming
                    ? aimDistance
                    : distance;

            currentHeight =
                isAiming
                    ? aimHeight
                    : height;
        }

        Quaternion orbitRotation =
            Quaternion.Euler(
                pitch,
                yaw,
                0f
            );

        Vector3 targetPosition =
            target.position +
            Vector3.up *
            currentHeight;

        Vector3 desiredPosition =
            targetPosition -
            orbitRotation *
            Vector3.forward *
            currentDistance;

        // Aim camera shoulder offset.
        if (isAiming &&
            !isStabilized)
        {
            desiredPosition +=
                orbitRotation *
                Vector3.right *
                aimSideOffset;
        }

        // --------------------------------
        // SMOOTH POSITION
        // --------------------------------

        transform.position =
            Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref currentVelocity,
                positionSmoothTime
            );

        // --------------------------------
        // LOOK AT TARGET
        // --------------------------------

        Quaternion desiredRotation =
            Quaternion.LookRotation(
                targetPosition -
                transform.position
            );

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                desiredRotation,
                rotationSmoothSpeed *
                Time.deltaTime
            );
    }
}