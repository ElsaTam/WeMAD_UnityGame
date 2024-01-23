using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : AttackAction
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



    public override string GetActionName() => "Sword";

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

    protected override void Init()
    {
        state = State.SwingingSwordBeforeHit;
        float beforeHitStateTime = .7f;
        stateTimer = beforeHitStateTime;
        OnSwordActionStarted?.Invoke(this, EventArgs.Empty);
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
                BattleSystem.Instance.PerformAttack(unit, targetUnit);
                break;
            case State.SwingingSwordAfterHit:
                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        };
    }

}
