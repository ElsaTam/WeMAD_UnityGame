using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    private GridSystem<GridObject> gridSystem;
    private GridPosition gridPosition;
    private List<Unit> unitList;
    private IInteractable interactable;

    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        this.unitList = new List<Unit>();
    }

    public void AddUnit(Unit unit)
    {
        this.unitList.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        this.unitList.Remove(unit);
    }

    public List<Unit> GetUnitList() => unitList;
    public bool IsEmpty() => unitList.Count == 0;
    public bool HasAnyUnit() => unitList.Count > 0;

    public Unit GetUnit()
    {
        if (HasAnyUnit()) return unitList[0];
        return null;
    }

    public IInteractable GetInteractable() => interactable;
    public void SetInteractable(IInteractable interactable)
    {
        this.interactable = interactable;
    }

    public override string ToString()
    {
        string unitString = "";
        foreach(Unit unit in unitList)
        {
            unitString += "\n" + unit;
        }
        return gridPosition.ToString() + unitString;
    }
}
