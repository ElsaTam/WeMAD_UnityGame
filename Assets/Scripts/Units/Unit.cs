using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyFriendlyActionPointsEmpty;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    public static event EventHandler OnAnyMouseEnter;
    public static event EventHandler OnAnyMouseExit;


    private UnitInfo unitInfo;
    private IWeapon weapon;

    private BaseAction[] baseActionArray;
    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private AttackInfo lastAttackInfo;
    private int actionPoints;

    private void Awake()
    {
        baseActionArray = GetComponents<BaseAction>();
        healthSystem = GetComponent<HealthSystem>();
        unitInfo = GetComponent<UnitInfo>();
        switch (unitInfo.GetClass())
        {
            case UnitInfo.UnitClass.Brawler:
                weapon = new WeaponHands();
                break;
            case UnitInfo.UnitClass.Fighter:
                weapon = new WeaponSword();
                break;
            case UnitInfo.UnitClass.Pistolero:
                weapon = new WeaponGun();
                break;
            case UnitInfo.UnitClass.Sniper:
                weapon = new WeaponSniper();
                break;
            case UnitInfo.UnitClass.Mage:
                weapon = new WeaponArtifact();
                break;
        }
        actionPoints = unitInfo.GetActionPointsMax();
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        healthSystem.OnDead += HealthSystem_OnDead;

        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        transform.position = LevelGrid.Instance.GetWorldPosition(gridPosition);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition) {
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;
            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
        }
    }

    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            if (baseAction is T) return (T)baseAction;
        }
        return null;
    }

    public BaseAction GetAttackAction()
    {
        switch (weapon.GetType())
        {
            case IWeapon.WeaponTypes.Hands:
                return GetAction<SwordAction>();
            case IWeapon.WeaponTypes.Sword:
                return GetAction<SwordAction>();
            case IWeapon.WeaponTypes.Gun:
                return GetAction<ShootAction>();
            case IWeapon.WeaponTypes.Sniper:
                return GetAction<ShootAction>();
            case IWeapon.WeaponTypes.Artifact:
                return GetAction<ShootAction>();
        }
        return null;
    }

    public UnitInfo GetUnitInfo() => unitInfo;
    public IWeapon GetWeapon() => weapon;

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public Vector3 GetAttackTargetPosition(float heightPercentage = 0.75f)
    {
        Bounds bounds = GetComponent<BoxCollider>().bounds;
        return new Vector3(
            bounds.center.x,
            bounds.min.y * (1f - heightPercentage) + bounds.max.y * heightPercentage,
            bounds.center.z
        );
    }

    public Vector3 GetOffsetTargetPosition(float heightPercentage = 0.75f)
    {
        return GetAttackTargetPosition(heightPercentage) - transform.position;
    }

    public AttackInfo GetLastAttackInfo()
    {
        return lastAttackInfo;
    }

    public void Damage(int damageAmount, Vector3 from)
    {
        lastAttackInfo.damage = damageAmount;
        lastAttackInfo.direction = (GetAttackTargetPosition() - from).normalized;
        healthSystem.TakeDamage(damageAmount);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        Destroy(gameObject);
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized() => healthSystem.GetHealthNormalized();
    public int GetHealth() => healthSystem.GetHealth();

    public bool TrySpendActionPoint(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction)) {
            SpendActionPoints(baseAction.GetActionPointCost());
            return true;
        }
        return false;
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        return actionPoints >= baseAction.GetActionPointCost();
    }

    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;
        if (actionPoints == 0 && !unitInfo.IsEnemy())
        {
            OnAnyFriendlyActionPointsEmpty?.Invoke(this, EventArgs.Empty);
        }
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (unitInfo.IsEnemy() && TurnSystem.Instance.IsPlayerTurn()) return;
        if (!unitInfo.IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) return;

        actionPoints = unitInfo.GetActionPointsMax();
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }





    public void OnPointerEnter(PointerEventData data)
    {
        OnAnyMouseEnter?.Invoke(this, EventArgs.Empty);
    }
    public void OnPointerExit(PointerEventData data)
    {
        OnAnyMouseExit?.Invoke(this, EventArgs.Empty);
    }

}
