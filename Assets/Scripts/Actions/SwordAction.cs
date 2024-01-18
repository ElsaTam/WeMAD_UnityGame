using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{

    public static event EventHandler OnAnySwordHit;
    public event EventHandler OnSwordActionStarted;
    public event EventHandler OnSwordActionCompleted;

    private enum State {
        SwingingSwordBeforeHit,
        SwingingSwordAfterHit
    }
    private State state;
    private float stateTimer;
    private Unit targetUnit;



    public override string GetActionName()
    {
        return "Sword";
    }

    private void Update()
    {
        if (!isActive) return;

        switch(state)
        {
            case State.SwingingSwordBeforeHit:
                Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
                break;
            case State.SwingingSwordAfterHit:
                break;
        };

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            NextState();
        }
    }
    
    private void NextState()
    {
        switch(state)
        {
            case State.SwingingSwordBeforeHit:
                state = State.SwingingSwordAfterHit;
                float afterHitStateTime = .1f;
                stateTimer = afterHitStateTime;
                OnAnySwordHit?.Invoke(this, EventArgs.Empty);
                targetUnit.Damage(unit.GetUnitInfo().GetFightDamage(), unit.GetAttackTargetPosition());
                break;
            case State.SwingingSwordAfterHit:
                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        };
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        EnemyAIAction enemyAIAction = new EnemyAIAction { gridPosition = gridPosition };
        enemyAIAction.actionValue = enemyAIAction.GetSwordActionValue();
        return enemyAIAction;
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validActionGridPositionList = new List<GridPosition>();

        UnitInfo unitInfo = unit.GetUnitInfo();
        for (int x = -unitInfo.GetMaxSwordDistance(); x <= unitInfo.GetMaxSwordDistance(); ++x)
        {
            for (int z = -unitInfo.GetMaxSwordDistance(); z <= unitInfo.GetMaxSwordDistance(); ++z)
            {
                GridPosition testGridPosition = unitGridPosition + new GridPosition(x, z);

                if (! LevelGrid.Instance.IsValidPosition(testGridPosition))          continue; // check if inside grid
                if (! LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) continue; // check if unit is on cell
                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                if (targetUnit.GetUnitInfo().IsEnemy() == unitInfo.IsEnemy())        continue; // check if unit is in opposite team
                
                validActionGridPositionList.Add(testGridPosition);
            }
        }

        return validActionGridPositionList;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        return GetValidActionGridPositionList(unit.GetGridPosition());
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.SwingingSwordBeforeHit;
        float beforeHitStateTime = .7f;
        stateTimer = beforeHitStateTime;

        OnSwordActionStarted?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
        
    }
}
