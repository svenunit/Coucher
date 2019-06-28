using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Transform gridLowerLeftCornerTransform;
    private Vector2 gridLowerLeftCorner;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float nodeSize = 1f;

    public Node[,] Map { get; private set; }

    public static PathfindingGrid Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CreateGrid();
    }

    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        var gridSize = new Vector3(width, height, 0f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(gridLowerLeftCornerTransform.position + (gridSize * .5f), gridSize);
        if (Map == null) return;
        Gizmos.color = Color.black;
        foreach (var node in Map)
        {
            Gizmos.DrawWireCube(node.WorldSpacePosition, new Vector3(nodeSize - .1f, nodeSize - .1f, nodeSize - .1f));
        }
    }

    private void CreateGrid()
    {
        gridLowerLeftCorner = gridLowerLeftCornerTransform.position + (.5f * new Vector3(nodeSize, nodeSize, 0f));
        Map = new Node[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldSpacePosition = gridLowerLeftCorner + Vector2.right * x * nodeSize + Vector2.up * y * nodeSize;
                Map[x, y] = new Node(worldSpacePosition, x, y, true);
            }
        }
    }

    public Node GetNodeByPosition(Vector2 worldSpacePos)
    {
        int x = Mathf.RoundToInt(worldSpacePos.x / nodeSize);
        int y = Mathf.RoundToInt(worldSpacePos.y / nodeSize);
        x = Mathf.Clamp(x, 0, width - 1);
        y = Mathf.Clamp(y, 0, height - 1);
        return Map[x, y];
    }

    public List<Node> GetNeighbours8(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                int nodeX = node.GridX + x;
                int nodeY = node.GridY + y;
                if ((nodeX >= 0 && nodeX <= width - 1) && (nodeY >= 0 && nodeY <= height - 1))
                    neighbours.Add(Map[nodeX, nodeY]);
            }
        }
        return neighbours;
    }
}
