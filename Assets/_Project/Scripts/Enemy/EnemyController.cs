using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Type")]
    [SerializeField] private EnemyType enemyType = EnemyType.Sword;

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 20f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Archer Movement")]
    [SerializeField] private float archerPreferredDistance = 12f;
    [SerializeField] private float archerMinimumDistance = 7f;
    [SerializeField] private float archerRetreatDistance = 6f;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError(
                "EnemyController: NavMeshAgent is missing!"
            );

            return;
        }

        agent.speed = moveSpeed;

        SetupEnemyType();
    }

    private void Start()
    {
        if (target == null)
        {
            GameObject arjuna =
                GameObject.FindGameObjectWithTag("Player");

            if (arjuna != null)
            {
                target = arjuna.transform;
            }
            else
            {
                Debug.LogError(
                    "EnemyController: No object with Player tag found."
                );
            }
        }
    }

    private void Update()
    {
        if (target == null || agent == null)
            return;

        // Arjuna is dead
        if (!target.gameObject.activeInHierarchy)
        {
            StopMoving();
            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            target.position
        );

        // Outside detection range
        if (distance > detectionRange)
        {
            StopMoving();
            return;
        }

        if (enemyType == EnemyType.Archer)
        {
            HandleArcherMovement(distance);
        }
        else
        {
            HandleMeleeMovement(distance);
        }
    }

    private void HandleMeleeMovement(float distance)
    {
        if (distance > agent.stoppingDistance)
        {
            ChaseTarget();
        }
        else
        {
            StopMoving();
        }
    }

    private void HandleArcherMovement(float distance)
    {
        // Arjuna is TOO CLOSE
        if (distance < archerMinimumDistance)
        {
            RetreatFromTarget();
        }

        // Arjuna is too far away
        else if (distance > archerPreferredDistance)
        {
            ChaseTarget();
        }

        // Perfect distance
        else
        {
            StopMoving();
        }
    }

    private void ChaseTarget()
    {
        if (!agent.isOnNavMesh)
            return;

        agent.isStopped = false;

        agent.SetDestination(target.position);
    }

    private void RetreatFromTarget()
    {
        if (!agent.isOnNavMesh)
            return;

        Vector3 directionAway =
            transform.position - target.position;

        directionAway.y = 0f;

        // Safety check
        if (directionAway.sqrMagnitude < 0.01f)
        {
            directionAway = -transform.forward;
        }

        directionAway.Normalize();

        // Move directly away from Arjuna
        Vector3 retreatPosition =
            transform.position +
            directionAway * archerRetreatDistance;

        NavMeshHit hit;

        bool foundPosition =
            NavMesh.SamplePosition(
                retreatPosition,
                out hit,
                10f,
                NavMesh.AllAreas
            );

        if (foundPosition)
        {
            agent.isStopped = false;
            agent.SetDestination(hit.position);
        }
        else
        {
            // If the exact retreat point isn't valid,
            // try several alternative directions.
            TryAlternativeRetreat();
        }
    }

    private void TryAlternativeRetreat()
    {
        Vector3[] directions =
        {
            transform.right,
            -transform.right,
            -transform.forward
        };

        foreach (Vector3 direction in directions)
        {
            Vector3 testPosition =
                transform.position +
                direction * archerRetreatDistance;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(
                testPosition,
                out hit,
                10f,
                NavMesh.AllAreas))
            {
                agent.isStopped = false;
                agent.SetDestination(hit.position);

                return;
            }
        }

        StopMoving();
    }

    private void StopMoving()
    {
        if (!agent.isOnNavMesh)
            return;

        agent.isStopped = true;
        agent.ResetPath();
    }

    private void SetupEnemyType()
    {
        switch (enemyType)
        {
            case EnemyType.Sword:

                agent.stoppingDistance = 2f;

                break;

            case EnemyType.Spear:

                agent.stoppingDistance = 3.5f;

                break;

            case EnemyType.Archer:

                agent.stoppingDistance =
                    archerPreferredDistance;

                break;
        }
    }
}