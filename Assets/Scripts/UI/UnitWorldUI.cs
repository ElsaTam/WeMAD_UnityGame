using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private Unit unit;
    
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private TextMeshProUGUI actionPointsText;

    [SerializeField] private Image healthBarImage;
    [SerializeField] private HealthSystem healthSystem;

    [SerializeField] private Transform unitInfoItemPrefab;
    [SerializeField] private Transform unitInfoContainerTransform;


    private List<UnitInfoItemUI> unitInfoItemUIList;

    private void Awake()
    {
        unitInfoItemUIList = new List<UnitInfoItemUI>();
        unitInfoContainerTransform.gameObject.SetActive(false);
    }

    private void Start()
    {
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        unit.OnMouseEnter += Unit_OnMouseEnter;
        unit.OnMouseExit += Unit_OnMouseExit;

        CreateUnitInfoItems();

        UpdateActionPointsText();
        UpdateHealthBar();
        unitNameText.text = unit.GetUnitInfo().GetUnitName();
    }

    private void CreateUnitInfoItems()
    {
        foreach(Transform unitInfoItem in unitInfoContainerTransform)
        {
            Destroy(unitInfoItem.gameObject);
        }
        unitInfoItemUIList.Clear();

        UnitInfo unitInfo = unit.GetUnitInfo();
        
        AddInfoItem("Movement", unitInfo.GetMaxMoveDistance());
        AddInfoItem("Fight dmg", unitInfo.GetFightDamage());
        AddInfoItem("Shoot dmg", unitInfo.GetShootDamage());
    }

    private void AddInfoItem(string name, int value)
    {
        Transform unitInfoItemTransform = Instantiate(unitInfoItemPrefab, unitInfoContainerTransform);
        UnitInfoItemUI unitInfoItemUI = unitInfoItemTransform.GetComponent<UnitInfoItemUI>();
        unitInfoItemUI.SetName(name);
        unitInfoItemUI.SetValue(value);
        unitInfoItemUI.Update();
        unitInfoItemUIList.Add(unitInfoItemUI);
    }










    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }

    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }

    private void Unit_OnMouseEnter(object sender, EventArgs e)
    {
        Debug.Log("UnitWorldUI.Unit_OnMouseEnter");
        unitInfoContainerTransform.gameObject.SetActive(true);
    }

    private void Unit_OnMouseExit(object sender, EventArgs e)
    {
        unitInfoContainerTransform.gameObject.SetActive(false);
    }

    private void UpdateActionPointsText()
    {
        actionPointsText.text = unit.GetActionPoints().ToString();
    }

    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
    }
}
