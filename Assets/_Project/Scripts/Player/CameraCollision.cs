using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Collision")]
    [SerializeField] private float collisionRadius = 0.2f;
    [SerializeField] private float collisionOffset = 0.15f;

    [Header("Collision Layers")]
    [SerializeField] private LayerMask collisionLayers = ~0;

    private Transform targetRoot;

    private void Awake()
    {
        if (target != null)
        {
            targetRoot = target.root;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 direction =
            transform.position -
            target.position;

        float distance =
            direction.magnitude;

        if (distance <= 0.01f)
            return;

        direction.Normalize();

        // Find everything between the target and camera.
        RaycastHit[] hits =
            Physics.SphereCastAll(
                target.position,
                collisionRadius,
                direction,
                distance,
                collisionLayers,
                QueryTriggerInteraction.Ignore
            );

        float closestDistance = distance;
        bool foundCollision = false;

        foreach (RaycastHit hit in hits)
        {
            // Ignore anything belonging to the player/chariot.
            if (targetRoot != null &&
                hit.collider.transform.root == targetRoot)
            {
                continue;
            }

            // Ignore the camera itself.
            if (hit.collider.transform == transform)
            {
                continue;
            }

            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                foundCollision = true;
            }
        }

        if (foundCollision)
        {
            transform.position =
                target.position +
                direction *
                Mathf.Max(
                    0f,
                    closestDistance -
                    collisionOffset
                );
        }
    }
}