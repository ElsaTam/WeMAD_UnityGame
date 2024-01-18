using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }

    [SerializeField] private Transform gridDebugObjectPrefab;

    public event EventHandler OnAnyUnitMoveGridPosition;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int cellSize = 2;
    private GridSystem<GridObject> gridSystem;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one LevelGrid. " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gridSystem = new GridSystem<GridObject>(
                        width, height, cellSize,
                        (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition)
                    );
        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab); // DEBUG
    }
    
    private void Start()
    {
        PathFinding.Instance.Setup(width, height, cellSize);
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        gridSystem.GetGridObject(gridPosition)?.AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        return gridSystem.GetGridObject(gridPosition)?.GetUnitList();
    }

    public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
    {
        return gridSystem.GetGridObject(gridPosition)?.GetInteractable();
    }

    public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
    {
        gridSystem.GetGridObject(gridPosition)?.SetInteractable(interactable);
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        gridSystem.GetGridObject(gridPosition)?.RemoveUnit(unit);
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit);
        AddUnitAtGridPosition(toGridPosition, unit);
        OnAnyUnitMoveGridPosition?.Invoke(this, EventArgs.Empty);
    }

    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
    {
        return ! gridSystem.GetGridObject(gridPosition).IsEmpty();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        return gridSystem.GetGridObject(gridPosition).GetUnit();
    }

    
    // Pass through functions
    public int GetWidth() => gridSystem.GetWidth();
    public int GetHeight() => gridSystem.GetHeight();
    public int GetCellSize() => gridSystem.GetCellSize();
    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
    public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);
    public bool IsValidPosition(GridPosition gridPosition) => gridSystem.IsValidPosition(gridPosition);
}
