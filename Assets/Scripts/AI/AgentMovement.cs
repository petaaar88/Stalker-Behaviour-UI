using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AgentMovement : MonoBehaviour
{
    [HideInInspector]
    public PathSolver pathSolver;
    public Transform target;
    public List<Node> previousPath = null;
    private List<Vector3> nodesPositions = new List<Vector3>();
    public int currentNodeIndex = 0;
    private bool isEnabled = true;

    public float speed = 2.0f;
    public float baseOffset = 0.0f;
    private float previousBaseOffset = 0.0f;
    public float stoppingDistance = 0.0f;
    public float rotationSpeed = 1.0f;

    // Collision Avoidance
    [Header("Collision Avoidance")]
    public bool useCollisionAvoidance = true;
    public float avoidanceBlendFactor = 0.7f; // Koliko utice avoidance na finalni pravac (0-1)
    private CollisionAvoidance collisionAvoidance;

    // Debugging
    public bool isUsingAStarDebug = false;

    private void Awake()
    {
        pathSolver = GetComponent<PathSolver>();
        collisionAvoidance = GetComponent<CollisionAvoidance>();

        if (collisionAvoidance == null && useCollisionAvoidance)
            collisionAvoidance = gameObject.AddComponent<CollisionAvoidance>();
        
    }

    void Start()
    {
        pathSolver.SetSeeker(transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null || !isEnabled)
            return;

        // Setting new base offset
        if (previousBaseOffset != baseOffset)
            SetNewBaseOffset();

        // Setting new path
        if (previousPath != pathSolver.path)
            SetNewPath();

        if (Vector3.Distance(transform.position, target.position) <= stoppingDistance - 0.6f) // 0.6f offset 
            return;

        // Move agent
        Vector3 direction = pathSolver.grid.NodeFromWorldPoint(target.position).worldPosition - transform.position;
        float distance = direction.magnitude;

        if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, distance, pathSolver.grid.unwalkableMask))
        {
            isUsingAStarDebug = true;
            pathSolver.canFindPath = true;
            UsePathfinding();
        }
        else
        {
            isUsingAStarDebug = false;
            pathSolver.canFindPath = false;
            GoStraightToTarget(direction);
        }
    }

    private void UsePathfinding()
    {
        if (nodesPositions == null || currentNodeIndex >= nodesPositions.Count)
            return;

        Vector3 targetPos = nodesPositions[currentNodeIndex];
        Vector3 direction = targetPos - transform.position;

        // Ako smo dovoljno blizu, predji na sledeci waypoint
        if (direction.sqrMagnitude <= 0.25f) // 0.5f^2
        {
            currentNodeIndex++;
            return;
        }

        // Primeni collision avoidance
        if (useCollisionAvoidance && collisionAvoidance != null)
        {
            Vector3 originalDirection = direction.normalized;
            Vector3 avoidanceDirection = collisionAvoidance.GetModifiedDirection(originalDirection);

            // Blend originalni i avoidance pravac
            direction = Vector3.Lerp(originalDirection, avoidanceDirection, avoidanceBlendFactor);
        }

        // Rotacija
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion smoothRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, smoothRotation.eulerAngles.y, 0);
        }

        // Kretanje - koristimo blendovani pravac ali čuvamo originalnu brzinu ka cilju
        Vector3 moveDirection = direction.normalized;
        Vector3 targetPosition = transform.position + moveDirection * speed * Time.deltaTime;

        // Proveri da li se još uvek krecemo prema cilju (sprecava "zaglavljene" agente)
        Vector3 toOriginalTarget = (targetPos - transform.position).normalized;
        if (Vector3.Dot(moveDirection, toOriginalTarget) > 0.3f) // Minimum 30% progresa prema cilju
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
        else
        {
            // Fallback: kreiraj novi put ako smo predaleko skrenuli
            pathSolver.canFindPath = true;
        }
    }

    private void GoStraightToTarget(Vector3 direction)
    {
        Vector3 targetPosition = pathSolver.grid.NodeFromWorldPoint(target.position).worldPosition;
        targetPosition.y = transform.position.y;

        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        // Primeni collision avoidance i na direktno kretanje
        if (useCollisionAvoidance && collisionAvoidance != null)
        {
            Vector3 avoidanceDirection = collisionAvoidance.GetModifiedDirection(moveDirection);
            moveDirection = Vector3.Lerp(moveDirection, avoidanceDirection, avoidanceBlendFactor);
        }

        transform.position = Vector3.MoveTowards(transform.position, transform.position + moveDirection * speed * Time.deltaTime, speed * Time.deltaTime);

        // Rotate towards modified direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            Quaternion smoothRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, smoothRotation.eulerAngles.y, 0);
        }
    }

    private void SetNewBaseOffset()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + baseOffset - previousBaseOffset, transform.position.z);

        if (nodesPositions != null)
            for (int i = 0; i < nodesPositions.Count; i++)
            {
                float offsetDelta = baseOffset - previousBaseOffset;
                Vector3 pos = nodesPositions[i];
                pos.y += offsetDelta;
                nodesPositions[i] = pos;
            }

        previousBaseOffset = baseOffset;
    }

    private void SetNewPath()
    {
        previousPath = pathSolver.path;
        currentNodeIndex = 0;
        nodesPositions.Clear();

        foreach (Node n in pathSolver.path)
        {
            // Adding base offset to nodes in path
            Vector3 position = n.worldPosition;
            position.y += baseOffset;
            nodesPositions.Add(position);
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        pathSolver.canFindPath = true;
        pathSolver.SetTarget(target);
    }

    public void EnableCollisionAvoidance()
    {
        useCollisionAvoidance = true;
        if (collisionAvoidance == null)
        {
            collisionAvoidance = gameObject.AddComponent<CollisionAvoidance>();
        }
    }

    public void DisableCollisionAvoidance()
    {
        useCollisionAvoidance = false;
    }

    public void SetAvoidanceBlendFactor(float factor)
    {
        avoidanceBlendFactor = Mathf.Clamp01(factor);
    }

    public void Enable()
    {
        isEnabled = true;
    }

    public void Disable()
    {
        isEnabled = false;
    }
}