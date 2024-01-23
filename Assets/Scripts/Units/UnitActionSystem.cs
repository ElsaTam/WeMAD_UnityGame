using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }

    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitsLayerMask;

    private BaseAction selectedAction;
    private bool isBusy;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one UnitActionSystem. " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Unit.OnAnyFriendlyActionPointsEmpty += Unit_OnAnyFriendlyActionPointsEmpty;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        SetSelectedUnit(selectedUnit);
    }

    private void Update()
    {
        if (InputManager.Instance.IsLeftMouseButtonUpThisFrame())
        {
            if (isBusy) return;
            if (! TurnSystem.Instance.IsPlayerTurn()) return; // Enemy turn
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (UIInteraction.Instance.GetPointedGUIElementList().Count > 0) return;// mouse on top of UI element
            }
            if (TryHandleUnitSelection()) return; // return if a unit was selected (don't move it)

            HandleSelectedAction();
        }
    }

    private bool TryHandleUnitSelection()
    {
        if (InputManager.Instance.IsLeftMouseButtonUpThisFrame())
        {

            Unit unit = GetPointedUnit(unitsLayerMask);
            if (unit)
            {
                if (selectedUnit == unit) return false; // unit already selected
                if (unit.GetUnitInfo().IsEnemy()) return false; // unit is an enemy
                
                SetSelectedUnit(unit);
                return true;
            }
        }
        return false;
    }

    private void HandleSelectedAction()
    {
        if (InputManager.Instance.IsLeftMouseButtonUpThisFrame())
        {
            // Select unit as target if it makes sense
            Unit targetUnit = null;
            if (selectedAction.NeedsUnitTarget())
            {
                targetUnit = GetPointedUnit(unitsLayerMask);
                if (targetUnit)
                {
                    if (! targetUnit.GetUnitInfo().IsEnemy()) targetUnit = null; // don't select if it's not an enemy
                    else if (! selectedAction.IsValidActionGridPosition(targetUnit.GetGridPosition())) targetUnit = null; // don't select if it's not a valid position
                }
            }

            // Find the grid position
            GridPosition actionGridPosition;
            if (targetUnit) // Either from the targeted unit
            {
                actionGridPosition = targetUnit.GetGridPosition();
            }
            else // Or directly from the targeted grid cell
            {
                actionGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
                if (! selectedAction.IsValidActionGridPosition(actionGridPosition)) return;
            }

            if (! selectedUnit.TrySpendActionPoint(selectedAction)) return;

            SetBusy();
            selectedAction.TakeAction(actionGridPosition, ClearBusy);

            OnActionStarted?.Invoke(this, EventArgs.Empty);
        }
    }

    private Unit GetPointedUnit(LayerMask unitsLayerMask)
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        if (Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, unitsLayerMask))
        {
            if (hitInfo.transform.TryGetComponent<Unit>(out Unit unit))
            {
                return unit;
            }
        }
        return null;
    }

    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        SetSelectedAction(unit.GetAction<MoveAction>()); // move action by default: all units must have a move action
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty); // only fire the event if we have subscribers
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty); // only fire the event if we have subscribers
    }

    private void SetBusy()
    {
        isBusy = true;
        OnBusyChanged?.Invoke(this, isBusy); // only fire the event if we have subscribers
    }

    private void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(this, isBusy); // only fire the event if we have subscribers
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }

    private void Unit_OnAnyFriendlyActionPointsEmpty(object sender, EventArgs e)
    {
        List<Unit> unitList = UnitManager.Instance.GetFriendlyUnitList();

        foreach (Unit unit in unitList)
        {
            if (unit.GetActionPoints() > 0) return;
        }

        TurnSystem.Instance.NextTurn();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            if (selectedUnit == null)
            {
                if (UnitManager.Instance.GetFriendlyUnitList().Count > 0)
                {
                    SetSelectedUnit(UnitManager.Instance.GetFriendlyUnitList()[0]);
                }
                else
                {
                    SetSelectedUnit(null);
                }
            }
        }
    }
}
