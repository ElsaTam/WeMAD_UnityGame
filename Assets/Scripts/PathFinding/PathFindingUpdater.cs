using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingUpdater : MonoBehaviour
{
    private void Start()
    {
        DestructibleObjectBase.OnAnyDestroyed += DestructibleObjectBase_OnAnyDestroyed;
    }

    private void DestructibleObjectBase_OnAnyDestroyed(object sender, EventArgs e)
    {
        DestructibleObjectBase destructibleObject = sender as DestructibleObjectBase;

        foreach (GridPosition gridPosition in destructibleObject.GetGridPositionList())
        {
            PathFinding.Instance.SetIsWalkableGridPosition(gridPosition, true);
        }
    }
}
