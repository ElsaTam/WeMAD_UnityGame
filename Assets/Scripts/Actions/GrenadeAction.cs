using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    [SerializeField] private LayerMask obstacleLayerMask;

    public event EventHandler<OnThrowEventArgs> OnThrow;
    public class OnThrowEventArgs : EventArgs
    {
        public Unit shootingUnit;
        public GridPosition targetGridPosition;
    }

    private float damageRadius = 2f;


    public override string GetActionName()
    {
        return "Grenade";
    }

    private void Update()
    {
        if (!isActive) return;
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validActionGridPositionList = new List<GridPosition>();

        UnitInfo unitInfo = unit.GetUnitInfo();
        for (int x = -unitInfo.GetMaxThrowDistance(); x <= unitInfo.GetMaxThrowDistance(); ++x)
        {
            for (int z = -unitInfo.GetMaxThrowDistance(); z <= unitInfo.GetMaxThrowDistance(); ++z)
            {
                GridPosition testGridPosition = unitGridPosition + new GridPosition(x, z);

                if (! LevelGrid.Instance.IsValidPosition(testGridPosition))       continue; // check if inside grid
                if (Mathf.Abs(x) + Mathf.Abs(z) > unitInfo.GetMaxThrowDistance()) continue; // check distance
                
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
        OnThrow?.Invoke(this, new OnThrowEventArgs{
            shootingUnit = unit,
            targetGridPosition = gridPosition
        });
        ActionStart(onActionComplete);
    }

    public void GetTargetListAtPosition(GridPosition targetGridPosition, out List<Unit> unitList, out List<DestructibleObjectBase> destructibleObjectList)
    {
        unitList = new List<Unit>();
        destructibleObjectList = new List<DestructibleObjectBase>();
        Collider[] colliderArray = Physics.OverlapSphere(LevelGrid.Instance.GetWorldPosition(targetGridPosition), GetDamageRadius());
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent<Unit>(out Unit targetUnit))
            {
                unitList.Add(targetUnit);
            }
            if (collider.TryGetComponent<DestructibleObjectBase>(out DestructibleObjectBase destructibleObject))
            {
                destructibleObjectList.Add(destructibleObject);
            }
        }
    }

    public int GetOppositeUnitCountAtPosition(GridPosition gridPosition)
    {
        GetTargetListAtPosition(gridPosition, out List<Unit> targetUnits, out List<DestructibleObjectBase> destructibleObjectList);
        int oppositeTargetCount = 0;
        foreach (Unit targetUnit in targetUnits)
        {
            oppositeTargetCount += Convert.ToInt32(targetUnit.GetUnitInfo().IsEnemy() != unit.GetUnitInfo().IsEnemy());
        }
        return oppositeTargetCount;
    }

    private float GetDamageRadius()
    {
        return damageRadius * LevelGrid.Instance.GetCellSize();
    }

    public void OnGrenadeBehaviourComplete(Vector3 targetPosition)
    {
        Explode(targetPosition);
        ActionComplete();
    }

    private void Explode(Vector3 targetPosition)
    {
        GetTargetListAtPosition(
            LevelGrid.Instance.GetGridPosition(targetPosition),
            out List<Unit> targetUnits,
            out List<DestructibleObjectBase> destructibleObjectList
        );
        foreach (Unit targetUnit in targetUnits)
        {
            targetUnit.Damage(30, transform.position);
        }
        foreach (DestructibleObjectBase destructibleObject in destructibleObjectList)
        {
            destructibleObject.Damage();
        }
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        EnemyAIAction enemyAIAction = new EnemyAIAction { gridPosition = gridPosition };
        
        enemyAIAction.actionValue = enemyAIAction.GetGrenadeActionValueFactor() * GetOppositeUnitCountAtPosition(gridPosition);

        return enemyAIAction;
    }

}
