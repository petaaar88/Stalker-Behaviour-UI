using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAvoidance : MonoBehaviour
{
    [Header("Collision Detection Settings")]
    public LayerMask avoidanceMask = -1; // Maske objekata koje treba izbegavati
    public float detectionRadius = 2.0f;
    public float avoidanceRadius = 1.5f;
    public float detectionAngle = 90f; // Ugao detekcije ispred agenta
    public float raycastDistance = 3.0f;

    [Header("Avoidance Forces")]
    public float separationForce = 2.0f;
    public float avoidanceForce = 3.0f;
    public float wallAvoidanceForce = 4.0f;

    [Header("Advanced Settings")]
    public int raycastCount = 5; // Broj raycasta za wall avoidance
    public float maxAvoidanceAngle = 45f; // Maksimalni ugao skretanja
    public bool useFlowField = true; // Da li koristiti flow field za grupu agenata

    [Header("Debug")]
    public bool showDebugGizmos = false;

    private AgentMovement agentMovement;
    private Rigidbody rb;
    private List<Collider> nearbyObjects = new List<Collider>();
    private Vector3 currentAvoidanceVector = Vector3.zero;

    void Start()
    {
        agentMovement = GetComponent<AgentMovement>();
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.freezeRotation = true;
            rb.useGravity = false;
        }
    }

    public Vector3 CalculateAvoidanceVector(Vector3 desiredDirection)
    {
        Vector3 avoidanceVector = Vector3.zero;

        // 1. Wall/Obstacle Avoidance
        Vector3 wallAvoidance = CalculateWallAvoidance(desiredDirection);
        avoidanceVector += wallAvoidance * wallAvoidanceForce;

        // 2. Agent Separation
        Vector3 separation = CalculateSeparation();
        avoidanceVector += separation * separationForce;

        // 3. Dynamic Obstacle Avoidance
        Vector3 dynamicAvoidance = CalculateDynamicAvoidance(desiredDirection);
        avoidanceVector += dynamicAvoidance * avoidanceForce;

        // Ograniči maksimalni ugao skretanja
        if (avoidanceVector.magnitude > 0)
        {
            float angle = Vector3.Angle(desiredDirection, avoidanceVector);
            if (angle > maxAvoidanceAngle)
            {
                avoidanceVector = Vector3.Slerp(desiredDirection, avoidanceVector, maxAvoidanceAngle / angle).normalized;
            }
        }

        currentAvoidanceVector = avoidanceVector;
        return avoidanceVector;
    }

    private Vector3 CalculateWallAvoidance(Vector3 forward)
    {
        Vector3 avoidanceVector = Vector3.zero;

        for (int i = 0; i < raycastCount; i++)
        {
            float angle = Mathf.Lerp(-detectionAngle / 2, detectionAngle / 2, i / (float)(raycastCount - 1));
            Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * forward;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, rayDirection, out hit, raycastDistance, avoidanceMask))
            {
                // Računaj avoidance vektor na osnovu normale površine
                Vector3 avoidDirection = Vector3.Reflect(rayDirection, hit.normal);
                float weight = 1.0f - (hit.distance / raycastDistance); // Bliži objekti imaju veći uticaj
                avoidanceVector += avoidDirection * weight;

                if (showDebugGizmos)
                {
                    Debug.DrawRay(transform.position, rayDirection * hit.distance, Color.red);
                    Debug.DrawRay(hit.point, hit.normal, Color.blue);
                }
            }
            else if (showDebugGizmos)
            {
                Debug.DrawRay(transform.position, rayDirection * raycastDistance, Color.green);
            }
        }

        return avoidanceVector.normalized;
    }

    private Vector3 CalculateSeparation()
    {
        Vector3 separationVector = Vector3.zero;
        int count = 0;

        // Pronađi sve agente u blizini
        Collider[] nearbyAgents = Physics.OverlapSphere(transform.position, detectionRadius, avoidanceMask);

        foreach (Collider other in nearbyAgents)
        {
            if (other.gameObject == gameObject) continue;

            AgentMovement otherAgent = other.GetComponent<AgentMovement>();
            if (otherAgent != null)
            {
                Vector3 diff = transform.position - other.transform.position;
                float distance = diff.magnitude;

                if (distance > 0 && distance < avoidanceRadius)
                {
                    // Jača separacija za bliže agente
                    float weight = 1.0f - (distance / avoidanceRadius);
                    separationVector += diff.normalized * weight;
                    count++;
                }
            }
        }

        if (count > 0)
        {
            separationVector /= count;
            return separationVector.normalized;
        }

        return Vector3.zero;
    }

    private Vector3 CalculateDynamicAvoidance(Vector3 desiredDirection)
    {
        Vector3 avoidanceVector = Vector3.zero;

        // Detektuj pokretne objekte koji se kreću ka agentu
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, detectionRadius, avoidanceMask);

        foreach (Collider other in nearbyObjects)
        {
            if (other.gameObject == gameObject) continue;

            Rigidbody otherRb = other.GetComponent<Rigidbody>();
            if (otherRb != null && otherRb.velocity.magnitude > 0.1f)
            {
                // Predvidi buduću poziciju objekta
                Vector3 relativePosition = other.transform.position - transform.position;
                Vector3 relativeVelocity = otherRb.velocity - (rb != null ? rb.velocity : Vector3.zero);

                float timeToCollision = Vector3.Dot(relativePosition, relativeVelocity) / relativeVelocity.sqrMagnitude;

                if (timeToCollision > 0 && timeToCollision < 2.0f) // 2 sekunde look-ahead
                {
                    Vector3 futurePosition = other.transform.position + otherRb.velocity * timeToCollision;
                    Vector3 ourFuturePosition = transform.position + desiredDirection * agentMovement.speed * timeToCollision;

                    float futureDistance = Vector3.Distance(futurePosition, ourFuturePosition);

                    if (futureDistance < avoidanceRadius)
                    {
                        Vector3 avoidDirection = (ourFuturePosition - futurePosition).normalized;
                        float urgency = 1.0f - (timeToCollision / 2.0f);
                        avoidanceVector += avoidDirection * urgency;
                    }
                }
            }
        }

        return avoidanceVector.normalized;
    }

    // Metoda za integraciju sa postojećim AgentMovement sistemom
    public Vector3 GetModifiedDirection(Vector3 originalDirection)
    {
        Vector3 avoidance = CalculateAvoidanceVector(originalDirection);

        if (avoidance.magnitude > 0.1f)
        {
            // Kombinuj originalni i avoidance pravac
            Vector3 combinedDirection = (originalDirection + avoidance).normalized;
            return combinedDirection;
        }

        return originalDirection;
    }

    // Flow field za koordinaciju grupa agenata
    private Vector3 CalculateFlowField()
    {
        if (!useFlowField) return Vector3.zero;

        Vector3 averageDirection = Vector3.zero;
        int count = 0;

        Collider[] nearbyAgents = Physics.OverlapSphere(transform.position, detectionRadius * 1.5f, avoidanceMask);

        foreach (Collider other in nearbyAgents)
        {
            if (other.gameObject == gameObject) continue;

            AgentMovement otherAgent = other.GetComponent<AgentMovement>();
            if (otherAgent != null && otherAgent.target == agentMovement.target)
            {
                Vector3 otherDirection = (otherAgent.target.position - other.transform.position).normalized;
                averageDirection += otherDirection;
                count++;
            }
        }

        if (count > 0)
        {
            averageDirection /= count;
            return averageDirection * 0.3f; // Manji uticaj flow field-a
        }

        return Vector3.zero;
    }

    void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        // Prikaz detection radius-a
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Prikaz avoidance radius-a
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, avoidanceRadius);

        // Prikaz trenutnog avoidance vektora
        if (currentAvoidanceVector.magnitude > 0)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, currentAvoidanceVector * 2);
        }
    }
}