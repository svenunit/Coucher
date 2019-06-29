using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private PathfindingGrid pathfindingGrid;

    void Start()
    {
        pathfindingGrid = GetComponent<PathfindingGrid>();
    }

    public List<Vector2> FindPath(Node start, Node end)
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(start);
        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                    currentNode = openSet[i];
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == end)
            {
                return RetracePathVector2s(start, end);
            }

            foreach (Node neighbourNode in pathfindingGrid.GetNeighbours8(currentNode))
            {
                if (neighbourNode.Walkable == false || closedSet.Contains(neighbourNode) == true) continue;

                int newMovementCostToNeigbour = currentNode.GCost + GetDistance(currentNode, neighbourNode);
                if (newMovementCostToNeigbour < neighbourNode.GCost || openSet.Contains(neighbourNode) == false)
                {
                    neighbourNode.GCost = newMovementCostToNeigbour;
                    neighbourNode.HCost = GetDistance(neighbourNode, end);
                    neighbourNode.ParentNode = currentNode;

                    if (openSet.Contains(neighbourNode) == false) openSet.Add(neighbourNode);
                }
            }
        }
        return null;
    }

    public List<Node> RetracePathNodes(Node startNode, Node targetNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = targetNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.ParentNode;
        }
        path.Reverse();
        return path;
    }

    public List<Vector2> RetracePathVector2s(Node startNode, Node targetNode)
    {
        List<Vector2> path = new List<Vector2>();
        Node currentNode = targetNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode.WorldSpacePosition);
            currentNode = currentNode.ParentNode;
        }
        path.Reverse();
        return path;
    }

    public int GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int distanceY = Mathf.Abs(nodeA.GridY - nodeB.GridY);
        if (distanceX > distanceY)
            return (14 * distanceY + 10 * (distanceX - distanceY));
        else
            return (14 * distanceX + 10 * (distanceY - distanceX));

    }
}