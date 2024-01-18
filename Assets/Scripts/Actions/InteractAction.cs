using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
    public override string GetActionName()
    {
        return "Interact";
    }


    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        EnemyAIAction enemyAIAction = new EnemyAIAction { gridPosition = gridPosition };
        enemyAIAction.actionValue = enemyAIAction.GetInteractActionValue();
        return enemyAIAction;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validActionGridPositionList = new List<GridPosition>();

        UnitInfo unitInfo = unit.GetUnitInfo();
        for (int x = -unitInfo.GetMaxInteractDistance(); x <= unitInfo.GetMaxInteractDistance(); ++x)
        {
            for (int z = -unitInfo.GetMaxInteractDistance(); z <= unitInfo.GetMaxInteractDistance(); ++z)
            {
                GridPosition testGridPosition = unit.GetGridPosition() + new GridPosition(x, z);

                if (! LevelGrid.Instance.IsValidPosition(testGridPosition)) continue; // check if inside grid
                
                IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);
                if (interactable == null) continue; // check if there is an interactable object
                
                validActionGridPositionList.Add(testGridPosition);
            }
        }

        return validActionGridPositionList;
    }


    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);
        interactable.Interact(OnInteractionComplete);
        ActionStart(onActionComplete);
    }

    private void OnInteractionComplete()
    {
        ActionComplete();
    }
}
