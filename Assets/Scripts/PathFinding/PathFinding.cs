using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public static PathFinding Instance { get; private set; }

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstacleLayerMask;

    private GridSystem<PathNode> gridSystem;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one PathFinding. " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Setup(int width, int height, int cellSize)
    {
        gridSystem = new GridSystem<PathNode>(width, height, cellSize, (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab); // DEBUG

        for (int x = 0; x < width; ++x)
        {
            for (int z = 0; z < height; ++z)
            {
                GridPosition gridPosition = new(x, z);
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                float raycastOffsetDistance = 5f;
                if (Physics.Raycast(
                    worldPosition + Vector3.down * raycastOffsetDistance,
                    Vector3.up,
                    raycastOffsetDistance * 2,
                    obstacleLayerMask))
                {
                    GetNode(x, z).SetIsWalkable(false);
                }
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out float pathLength)
    {
        List<PathNode> openList = new List<PathNode>(); // nodes queued for searching
        List<PathNode> closedList = new List<PathNode>(); // nodes already searched

        PathNode startNode = gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);

        for (int x = 0; x < gridSystem.GetWidth(); ++x)
        {
            for (int z = 0; z < gridSystem.GetHeight(); ++z)
            {
                PathNode pathNode = gridSystem.GetGridObject(new GridPosition(x, z));
                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if (currentNode == endNode)
            {
                pathLength = endNode.GetFCost() / 10f;
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;
                if (! neighbourNode.IsWalkable())
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

                if (tentativeGCost < neighbourNode.GetGCost())
                {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.SetGCost(tentativeGCost);
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
                    neighbourNode.CalculateFCost();

                    if (! openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        pathLength = 0;
        return null;
    }

    public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        GridPosition gridPositionDistance = gridPositionA - gridPositionB;
        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistance - zDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];
        foreach(PathNode pathNode in pathNodeList)
        {
            if (pathNode.GetFCost() < lowestFCostPathNode.GetFCost())
            {
                lowestFCostPathNode = pathNode;
            }
        }
        return lowestFCostPathNode;
    }

    private PathNode GetNode(int x, int z)
    {
        return gridSystem.GetGridObject(new GridPosition(x, z));
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPosition();

        if (gridPosition.x - 1 >= 0)
        {
            neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0)); // left
            if (gridPosition.z - 1 >= 0)
            {
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1)); // left-down
            }
            if (gridPosition.z + 1 < gridSystem.GetHeight())
            {
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1)); // left-up
            }
        }
        if (gridPosition.x + 1 < gridSystem.GetWidth())
        {
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0)); // right
            if (gridPosition.z - 1 >= 0)
            {
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1)); // right-down
            }
            if (gridPosition.z + 1 < gridSystem.GetHeight())
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1)); // right-up
            {
            }
        }
        if (gridPosition.z - 1 >= 0)
        {
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1)); // down
        }
        if (gridPosition.z + 1 < gridSystem.GetHeight())
        {
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1)); // up
        }

        return neighbourList;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<GridPosition> pathGridPositionList = new List<GridPosition>();

        PathNode currentNode = endNode;
        while (currentNode != null)
        {
            pathGridPositionList.Add(currentNode.GetGridPosition());
            currentNode = currentNode.GetCameFromPathNode();
        }
        pathGridPositionList.Reverse();

        return pathGridPositionList;
    }


    public bool IsWalkableGridPosition(GridPosition gridPosition)
    {
        return gridSystem.GetGridObject(gridPosition).IsWalkable();
    }
    
    public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
    {
        gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }


    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition, out float pathLength) != null;
    }

    public float GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out float pathLength);
        return pathLength;
    }

}
