using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static List<GridPosition> GetGridPositionsCoveredByObject(Bounds bounds, int cellSize)
    {
        List<GridPosition> gridPositionList = new List<GridPosition> ();

        Vector3Int boundsMin = new(
            Mathf.CeilToInt(bounds.min.x),
            0,
            Mathf.CeilToInt(bounds.min.z)
        );
        Vector3Int boundsMax = new(
            Mathf.FloorToInt(bounds.max.x),
            0,
            Mathf.FloorToInt(bounds.max.z)
        );

        boundsMin.x += (cellSize - boundsMin.x % cellSize) % cellSize;
        boundsMin.z += (cellSize - boundsMin.z % cellSize) % cellSize;
        boundsMax.x -= boundsMax.x % cellSize;
        boundsMax.z -= boundsMax.z % cellSize;

        for (int x = boundsMin.x; x <= boundsMax.x; x += cellSize)
        {
            for (int z = boundsMin.z; z <= boundsMax.z; z += cellSize)
            {
                gridPositionList.Add(new GridPosition(x / cellSize, z / cellSize));
            }
        }

        return gridPositionList;
    }
}
