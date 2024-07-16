using UnityEngine;
public class Node
{
    public Vector3Int Position { get; set; }
    public bool IsWalkable { get; set; }
    public bool Occupied { get; set; }
    public Node Parent { get; set; }
    public float GCost { get; set; } // Cost from start to node
    public float HCost { get; set; } // Heuristic cost from node to goal
    public float FCost => GCost + HCost; // Total cost

    public Node(Vector3Int position, bool isWalkable)
    {
        Position = position;
        IsWalkable = isWalkable;
    }
}