using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ShootAction : BaseAction
{
    [SerializeField] private LayerMask obstacleLayerMask;

    private enum State {
        Aiming,
        Shooting,
        Cooloff
    }
    private State state;
    private float stateTimer;

    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    private Unit targetUnit;
    private bool canShootBullet;

    public override string GetActionName()
    {
        return "Shoot";
    }

    public override bool NeedsUnitTarget()
    {
        return true;
    }

    private void Update()
    {
        if (!isActive) return;

        switch(state)
        {
            case State.Aiming:
                Aim();
                break;
            case State.Shooting:
                if (canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.Cooloff:
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
            case State.Aiming:
                state = State.Shooting;
                float shootingStateTime = .1f;
                stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                state = State.Cooloff;
                float cooloffStateTime = .5f;
                stateTimer = cooloffStateTime;
                break;
            case State.Cooloff:
                ActionComplete();
                break;
        };
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validActionGridPositionList = new List<GridPosition>();

        UnitInfo unitInfo = unit.GetUnitInfo();
        for (int x = -unitInfo.GetMaxShootDistance(); x <= unitInfo.GetMaxShootDistance(); ++x)
        {
            for (int z = -unitInfo.GetMaxShootDistance(); z <= unitInfo.GetMaxShootDistance(); ++z)
            {
                GridPosition testGridPosition = unitGridPosition + new GridPosition(x, z);

                if (! LevelGrid.Instance.IsValidPosition(testGridPosition))          continue; // check if inside grid
                if (Mathf.Abs(x) + Mathf.Abs(z) > unitInfo.GetMaxShootDistance())    continue; // check distance
                if (! LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) continue; // check if unit is on cell
                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                if (targetUnit.GetUnitInfo().IsEnemy() == unitInfo.IsEnemy())        continue; // check if unit is in opposite team

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

        state = State.Aiming;
        float aimingStateTime = 0.3f;
        stateTimer = aimingStateTime;
        
        canShootBullet = true;

        ActionStart(onActionComplete);
    }

    private void Aim()
    {
        Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
        float rotateSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
    }

    private void Shoot()
    {
        OnAnyShoot?.Invoke(this, new OnShootEventArgs{
            targetUnit = targetUnit,
            shootingUnit = unit
        });
        OnShoot?.Invoke(this, new OnShootEventArgs{
            targetUnit = targetUnit,
            shootingUnit = unit
        });
        targetUnit.Damage(unit.GetUnitInfo().GetShootDamage(), unit.GetAttackTargetPosition());
    }

    public Unit GetTargetUnit()
    {
        return targetUnit;
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        EnemyAIAction enemyAIAction = new EnemyAIAction { gridPosition = gridPosition };

        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        enemyAIAction.actionValue = enemyAIAction.GetShootActionValueMin() + (100 - Mathf.RoundToInt(targetUnit.GetHealthNormalized() * 100));
        return enemyAIAction;
    }

}
