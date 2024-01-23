using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public abstract class AttackAction : BaseAction
{
    [SerializeField] private LayerMask obstacleLayerMask;


    protected Unit targetUnit;

    public override bool NeedsUnitTarget()
    {
        return true;
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validActionGridPositionList = new List<GridPosition>();

        UnitInfo unitInfo = unit.GetUnitInfo();
        IWeapon weapon = unit.GetWeapon();

        for (int x = -weapon.GetRange(); x <= weapon.GetRange(); ++x)
        {
            for (int z = -weapon.GetRange(); z <= weapon.GetRange(); ++z)
            {
                GridPosition testGridPosition = unitGridPosition + new GridPosition(x, z);

                if (! LevelGrid.Instance.IsValidPosition(testGridPosition)) continue; // check if inside grid
                
                if (Mathf.Abs(x) + Mathf.Abs(z) > weapon.GetRange()) continue; // check distance
                
                if (! LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) continue; // check if unit is on cell
                
                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                if (targetUnit.GetUnitInfo().IsEnemy() == unitInfo.IsEnemy()) continue; // check if unit is in opposite team

                Vector3 shootOrigin = LevelGrid.Instance.GetWorldPosition(unitGridPosition) + unit.GetOffsetTargetPosition();
                Vector3 shootTarget = targetUnit.GetAttackTargetPosition();
                bool blockedByObstacle = Physics.Raycast(
                    shootOrigin,
                    (shootTarget - shootOrigin).normalized,
                    Vector3.Distance(shootOrigin, shootTarget),
                    obstacleLayerMask
                );
                if(blockedByObstacle) continue; // check if obstacle is in the way
                
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
        Init();
        ActionStart(onActionComplete);
    }

    protected abstract void Init();

    protected void Aim()
    {
        Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
        float rotateSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
    }

    public Unit GetTargetUnit() => targetUnit;

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        EnemyAIAction enemyAIAction = new EnemyAIAction { gridPosition = gridPosition };

        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        enemyAIAction.actionValue = enemyAIAction.GetAttackActionValueMin() + (100 - Mathf.RoundToInt(targetUnit.GetHealthNormalized() * 100));
        return enemyAIAction;
    }

}
