using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private GridPosition gridPosition;
    private int gCost; // walking cost from the start node
    private int hCost; // heuristic cost to reach end node
    private int fCost; // g + h
    private PathNode cameFromPathNode;
    private bool isWalkable = true;

    public PathNode(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    public GridPosition GetGridPosition() => gridPosition;
    public PathNode GetCameFromPathNode() => cameFromPathNode;
    public int GetGCost() => gCost;
    public int GetHCost() => hCost;
    public int GetFCost() => fCost;
    public bool IsWalkable() => isWalkable;

    public void SetGCost(int gCost)
    {
        this.gCost = gCost;
    }
    public void SetHCost(int hCost)
    {
        this.hCost = hCost;
    }
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
    public void ResetCameFromPathNode()
    {
        cameFromPathNode = null;
    }
    public void SetCameFromPathNode(PathNode pathNode)
    {
        cameFromPathNode = pathNode;
    }
    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }

    public override string ToString()
    {
        return gridPosition.ToString();
    }
}
