using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem<TGridObject>
{

    private int width;
    private int height;
    private int cellSize;
    private TGridObject[,] gridObjectArray;

    public GridSystem(int width, int height, int cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridObjectArray = new TGridObject[width, height];

        for (int x = 0; x < width; ++x)
        {
            for (int z = 0; z < height; ++z)
            {
                gridObjectArray[x, z] = createGridObject(this, new GridPosition(x, z));
            }
        }
    }

    public int GetWidth() => width;
    public int GetHeight() => height;
    public int GetCellSize() => cellSize;

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(Mathf.RoundToInt(worldPosition.x / cellSize), Mathf.RoundToInt(worldPosition.z / cellSize));
    }

    public TGridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.x, gridPosition.z];
    }

    public bool IsValidPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < width && gridPosition.z >= 0 && gridPosition.z < height;
    }





    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < width; ++x)
        {
            for (int z = 0; z < height; ++z)
            {
                GridPosition gridPosition = new(x, z);
                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }

}
