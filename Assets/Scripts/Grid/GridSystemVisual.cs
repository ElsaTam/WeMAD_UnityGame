using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }


    public enum GridVisualType
    {
        White,
        Blue,
        Red,
        RedSoft,
        Yellow
    }
    [Serializable] public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }

    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;
    [SerializeField] private Transform planeGridPrefab;

    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    private GridPosition mouseGridPosition;



    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one GridSystemVisual. " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); ++x)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); ++z)
            {
                Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(new GridPosition(x, z)), Quaternion.identity);
                gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMoveGridPosition += LevelGrid_OnAnyUnitMoveGridPosition;
        planeGridPrefab.GetComponent<PlaneGrid>().OnMouseOverPlane += PlaneGrid_OnMouseOverPlane;

        UpdateGridVisual();
    }

    private void PlaneGrid_OnMouseOverPlane(object sender, EventArgs e)
    {
        GridPosition newMouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
        if (newMouseGridPosition == mouseGridPosition) return;

        mouseGridPosition = newMouseGridPosition;
        switch (UnitActionSystem.Instance.GetSelectedAction())
        {
            case GrenadeAction grenadeAction:
                UpdateGridVisual();
                break;
        }
        return;
    }

    private void UpdateGridVisual()
    {
        HideAllGridPosition();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        List<GridPosition> validGridPositionList = selectedAction.GetValidActionGridPositionList();
        GridVisualType gridVisualType;
        switch (selectedAction)
        {
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                break;
            case SpinAction spinAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(selectedUnit.GetGridPosition(), selectedUnit.GetWeapon().GetRange(), GridVisualType.RedSoft);
                break;
            case GrenadeAction grenadeAction:
                gridVisualType = GridVisualType.Yellow;

                grenadeAction.GetTargetListAtPosition(mouseGridPosition, out List<Unit> targetUnitList, out List<DestructibleObjectBase> destructibleObjectList);
                List<GridPosition> targetGridPositionList = new List<GridPosition>();
                foreach (Unit targetUnit in targetUnitList)
                {
                    targetGridPositionList.Add(targetUnit.GetGridPosition());
                    validGridPositionList.Remove(targetUnit.GetGridPosition());
                }
                foreach (DestructibleObjectBase destructibleObject in destructibleObjectList)
                {
                    foreach (GridPosition objectGridPosition in destructibleObject.GetGridPositionList())
                    {
                        targetGridPositionList.Add(objectGridPosition);
                        validGridPositionList.Remove(objectGridPosition);
                    }
                }
                ShowGridPositionList(targetGridPositionList, GridVisualType.Red);
                break;
            case SwordAction swordAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRangeSquare(selectedUnit.GetGridPosition(), selectedUnit.GetWeapon().GetRange(), GridVisualType.RedSoft);
                break;
            case InteractAction interactAction:
                gridVisualType = GridVisualType.Blue;
                break;
            default:
                gridVisualType = GridVisualType.White;
                break;
        }
        ShowGridPositionList(validGridPositionList, gridVisualType);
    }

    public void HideAllGridPosition()
    {
        foreach(GridSystemVisualSingle gridSystemVisualSingle in gridSystemVisualSingleArray)
        {
            gridSystemVisualSingle.Hide();
        }
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x <= range; ++x)
        {
            for (int z = -range; z <= range; ++z)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);
                if (! LevelGrid.Instance.IsValidPosition(testGridPosition)) continue; // check if inside grid
                if (Mathf.Abs(x) + Mathf.Abs(z) > range) continue; // check distance
                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    private void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x <= range; ++x)
        {
            for (int z = -range; z <= range; ++z)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);
                if (! LevelGrid.Instance.IsValidPosition(testGridPosition)) continue; // check if inside grid
                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
    {
        foreach(GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }
    
    private void LevelGrid_OnAnyUnitMoveGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType) return gridVisualTypeMaterial.material;
        }
        Debug.LogError("Could not find GridVisualTypeMaterial for GridVisualType " + gridVisualType);
        return null;
    }
}
