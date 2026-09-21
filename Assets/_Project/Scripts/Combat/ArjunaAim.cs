using UnityEngine;
using UnityEngine.InputSystem;

public class ArjunaAim : MonoBehaviour
{
    [Header("Aim UI")]
    [SerializeField] private GameObject crosshair;

    [Header("Aim Rotation")]
    [SerializeField] private float rotationSpeed = 15f;

    public bool IsAiming { get; private set; }

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        if (crosshair != null)
            crosshair.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Unlock mouse when Escape is pressed
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Mouse.current == null)
            return;

        // Right mouse = Aim
        IsAiming = Mouse.current.rightButton.isPressed;

        // Show/hide crosshair
        if (crosshair != null)
            crosshair.SetActive(IsAiming);

        // Rotate Arjuna toward aim direction
        if (IsAiming)
        {
            RotateTowardsAim();
        }
    }

    private void RotateTowardsAim()
    {
        if (mainCamera == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(
            new Vector3(
                Screen.width * 0.5f,
                Screen.height * 0.5f,
                0f
            )
        );

        Vector3 aimPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            aimPoint = hit.point;
        }
        else
        {
            aimPoint = ray.origin + ray.direction * 1000f;
        }

        Vector3 direction = aimPoint - transform.position;

        // Keep rotation horizontal
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}