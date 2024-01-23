using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitInfoUI : MonoBehaviour
{
    [SerializeField] private Transform unitInfoItemPrefab;
    [SerializeField] private Transform unitInfoContainerTransform;
    [SerializeField] private TextMeshProUGUI textSelectedUnitName;
    [SerializeField] private TextMeshProUGUI textTargetUnitName;

    private List<UnitInfoItemUI> unitInfoItemUIList;

    private void Awake()
    {
        unitInfoItemUIList = new List<UnitInfoItemUI>();
        ClearContainer();
    }

    private void Start()
    {
        Unit.OnAnyMouseEnter += Unit_OnAnyMouseEnter;
        Unit.OnAnyMouseExit += Unit_OnAnyMouseExit;
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        CreateUnitInfoItems(null);
    }

    private void ClearContainer()
    {
        foreach(Transform unitInfoItem in unitInfoContainerTransform)
        {
            Destroy(unitInfoItem.gameObject);
        }
        unitInfoItemUIList.Clear();
        textSelectedUnitName.text = "";
        textTargetUnitName.text = "";
    }

    private void ClearTargetUnitInfo()
    {
        foreach(UnitInfoItemUI unitInfoItemUI in unitInfoItemUIList)
        {
            unitInfoItemUI.RemoveValueTargetUnit();
        }
    }

    private void CreateUnitInfoItems(Unit targetUnit)
    {
        ClearContainer();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        textSelectedUnitName.text = selectedUnit.GetUnitInfo().GetUnitName();
        textTargetUnitName.text = targetUnit == null ? "" : targetUnit.GetUnitInfo().GetUnitName();
        
        AddInfoItem("HP",
                    selectedUnit.GetHealth().ToString(),
                    targetUnit == null ? "" : targetUnit.GetHealth().ToString());
        AddInfoItem("Hit rate",
                    BattleSystem.Instance.GetHitRate(selectedUnit).ToString(),
                    targetUnit == null ? "" : BattleSystem.Instance.GetHitRate(targetUnit).ToString());
        AddInfoItem("Evade",
                    BattleSystem.Instance.GetAvoid(selectedUnit).ToString(),
                    targetUnit == null ? "" : BattleSystem.Instance.GetAvoid(targetUnit).ToString());
        AddInfoItem("Crit. rate",
                    BattleSystem.Instance.GetCriticalRate(selectedUnit).ToString(),
                    targetUnit == null ? "" : BattleSystem.Instance.GetCriticalRate(targetUnit).ToString());
        AddInfoItem("Crit. evade",
                    BattleSystem.Instance.GetCriticalAvoid(selectedUnit).ToString(),
                    targetUnit == null ? "" : BattleSystem.Instance.GetCriticalAvoid(targetUnit).ToString());
        AddInfoItem("Attack",
                    BattleSystem.Instance.GetAttackPower(selectedUnit).ToString(),
                    targetUnit == null ? "" : BattleSystem.Instance.GetAttackPower(targetUnit).ToString());
        AddInfoItem("Physical def.",
                    BattleSystem.Instance.GetDefensePower(selectedUnit, IWeapon.WeaponTypes.Hands).ToString(),
                    targetUnit == null ? "" : BattleSystem.Instance.GetDefensePower(targetUnit, IWeapon.WeaponTypes.Hands).ToString());
        AddInfoItem("Magical def.",
                    BattleSystem.Instance.GetDefensePower(selectedUnit, IWeapon.WeaponTypes.Artifact).ToString(),
                    targetUnit == null ? "" : BattleSystem.Instance.GetDefensePower(targetUnit, IWeapon.WeaponTypes.Artifact).ToString());
    }

    private void AddInfoItem(string name, string valueSelectedUnit, string valueTargetUnit)
    {
        Transform unitInfoItemTransform = Instantiate(unitInfoItemPrefab, unitInfoContainerTransform);
        UnitInfoItemUI unitInfoItemUI = unitInfoItemTransform.GetComponent<UnitInfoItemUI>();
        unitInfoItemUI.SetName(name);
        unitInfoItemUI.SetValueSelectedUnit(valueSelectedUnit);
        unitInfoItemUI.SetValueTargetUnit(valueTargetUnit);
        unitInfoItemUIList.Add(unitInfoItemUI);
    }






    private void Unit_OnAnyMouseEnter(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        if (unit.GetUnitInfo().IsEnemy())
            CreateUnitInfoItems(sender as Unit);
    }

    private void Unit_OnAnyMouseExit(object sender, EventArgs e)
    {
        ClearTargetUnitInfo();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        CreateUnitInfoItems(null);
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            unitInfoContainerTransform.gameObject.SetActive(true);
        }
        else
        {
            unitInfoContainerTransform.gameObject.SetActive(false);
        }
    }
}
