using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShootAction : AttackAction
{
    
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

    private bool canShootBullet;

    public override string GetActionName() => "Shoot";

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

    protected override void Init()
    {
        state = State.Aiming;
        float aimingStateTime = 0.3f;
        stateTimer = aimingStateTime;
        canShootBullet = true;
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
        BattleSystem.Instance.PerformAttack(unit, targetUnit);
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

}
