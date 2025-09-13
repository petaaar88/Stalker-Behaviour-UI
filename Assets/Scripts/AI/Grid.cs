using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{

    public bool onlyDisplayPathGizmos;
    public bool showGrid;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    [Min(1)]
    public int blendFactor = 1;
    private Node[,] grid;
    public Dictionary<int, List<Node>> paths;
    private int pathsNextId = 0;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Start()
    {
        paths = new Dictionary<int, List<Node>>();
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius * blendFactor, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromAgentPosition(Vector3 seekerPosition)
    {
        float percentX = (seekerPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (seekerPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        Node node = grid[x, y];
        
        return node;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        Node node = grid[x, y];

        if (node.walkable)
            return node;

        // Sirimo se koncentricno dok ne pronađemo walkable cvor
        int maxRadius = Mathf.Max(gridSizeX, gridSizeY);
        for (int radius = 1; radius < maxRadius; radius++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    // Samo ivice trenutnog kvadrata (spiralna ivica)
                    if (Mathf.Abs(dx) != radius && Mathf.Abs(dy) != radius)
                        continue;

                    int checkX = x + dx;
                    int checkY = y + dy;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        Node neighbor = grid[checkX, checkY];
                        if (neighbor.walkable)
                        {
                            return neighbor;
                        }
                    }
                }
            }
        }

        // Ako nijedan walkable nije pronađen (sto ne bi trebalo da se desi), vracamo originalni
        return node;
    }

    public void AddNewPath(PathSolver pathSolver) { pathSolver.SetPathId(pathsNextId++); }

    public List<Node> path;
    
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (onlyDisplayPathGizmos)
        {
            foreach (KeyValuePair<int, List<Node>> pair in paths)
            {
                if(pair.Value != null)
                    foreach (Node n in pair.Value)
                    {
                        Gizmos.color = Color.green;

                        Vector3 nodeSize = Vector3.one * (nodeDiameter - .1f);
                        nodeSize.y = 0.1f;
                        Gizmos.DrawCube(n.worldPosition, nodeSize);
                    }
            }
        }
        else
        {
            if (!showGrid)
                return;

            if (grid != null)
            {
                foreach (Node n in grid)
                {
                    Gizmos.color = (n.walkable) ? new Color(66/(float) 255, 221 / (float)255, 245 / (float)255) : Color.clear;
                    foreach (KeyValuePair<int, List<Node>> pair in paths)
                        if(pair.Value != null)
                            if (pair.Value.Contains(n))
                                Gizmos.color = Color.green;

                    Vector3 nodeSize = Vector3.one * (nodeDiameter - .1f);
                    nodeSize.y = 0.1f;
                    Gizmos.DrawCube(n.worldPosition, nodeSize);
                }
            }
        }
    }
}