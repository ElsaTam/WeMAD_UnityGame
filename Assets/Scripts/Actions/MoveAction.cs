using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    private List<Vector3> positionList;
    private int currentPositionIndex;

    public override string GetActionName()
    {
        return "Move";
    }

    void Update()
    {
        if (!isActive) return;

        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

        float epsilonDistance = .1f;
        if (Vector3.Distance(targetPosition, transform.position) > epsilonDistance)
        {
            float moveSpeed = 4f;
            transform.position += Time.deltaTime * moveSpeed * moveDirection;
        }
        else {
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = PathFinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out float pathLength);

        currentPositionIndex = 0;
        positionList = new List<Vector3>();
        
        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        OnStartMoving?.Invoke(this, EventArgs.Empty);
        
        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validActionGridPositionList = new List<GridPosition>();
        UnitInfo unitInfo = unit.GetUnitInfo();

        GridPosition unitGridPosition = unit.GetGridPosition();
        for (int x = -unitInfo.GetMovement(); x <= unitInfo.GetMovement(); ++x)
        {
            for (int z = -unitInfo.GetMovement(); z <= unitInfo.GetMovement(); ++z)
            {
                GridPosition gridPosition = unitGridPosition + new GridPosition(x, z);

                if (! LevelGrid.Instance.IsValidPosition(gridPosition))            continue;
                if (unitGridPosition == gridPosition)                              continue;
                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(gridPosition))     continue;
                if (! PathFinding.Instance.IsWalkableGridPosition(gridPosition))   continue;
                float pathLength = PathFinding.Instance.GetPathLength(unitGridPosition, gridPosition);
                if (pathLength <= 0 || pathLength > unitInfo.GetMovement()) continue;
                
                validActionGridPositionList.Add(gridPosition);
            }
        }

        return validActionGridPositionList;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        EnemyAIAction enemyAIAction = new EnemyAIAction { gridPosition = gridPosition };

        int targetCount = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        if (targetCount == 0)
        {
            // Based on the distance to the closest opposite unit
            float shortestDistance = enemyAIAction.GetMoveToGetCloserActionValueMax();
            foreach (Unit oppositeUnit in UnitManager.Instance.GetFriendlyUnitList())
            {
                float distance = PathFinding.Instance.GetPathLength(gridPosition, oppositeUnit.GetGridPosition());
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                }
            }
            enemyAIAction.actionValue = enemyAIAction.GetMoveToGetCloserActionValueMax() - Mathf.RoundToInt(shortestDistance);
        }
        else
        {
            // Based on the number of possible targets
            enemyAIAction.actionValue = enemyAIAction.GetMoveToGetShootActionValueMin() + targetCount;
        }
        return enemyAIAction;
    }


}
