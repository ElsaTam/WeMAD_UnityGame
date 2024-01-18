using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    private float totalSpinAmount;

    public override string GetActionName()
    {
        return "Spin";
    }

    public override int GetActionPointCost()
    {
        return 1;
    }

    private void Update()
    {
        if (! isActive) return;
        
        float spinAddAmount = 360f * Time.deltaTime;
        bool fullTurn = false;
        if (totalSpinAmount + spinAddAmount > 360f) {
            spinAddAmount = 360f - totalSpinAmount;
            fullTurn = true;
        }
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0);
        totalSpinAmount += spinAddAmount;

        if (fullTurn) ActionComplete();
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        totalSpinAmount = 0f;
        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        return new List<GridPosition>{ unit.GetGridPosition() };
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        EnemyAIAction enemyAIAction = new EnemyAIAction { gridPosition = gridPosition };
        enemyAIAction.actionValue = enemyAIAction.GetSpinActionValue();
        return enemyAIAction;
    }
}
