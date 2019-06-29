using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector2 WorldSpacePosition { get; set; }
    public int GridX { get; set; }
    public int GridY { get; set; }

    public bool Walkable { get; set; }

    public float GCost { get; set; }
    public float HCost { get; set; }
    public float FCost => GCost + HCost;

    public Node ParentNode { get; set; }

    public Node(Vector2 worldSpacePosition, int gridX, int gridY, bool walkable)
    {
        WorldSpacePosition = worldSpacePosition;
        GridX = gridX;
        GridY = gridY;
        Walkable = walkable;
    }
}
