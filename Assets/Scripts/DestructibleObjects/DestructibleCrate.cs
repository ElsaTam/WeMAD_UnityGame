using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleCrate : DestructibleObjectBase
{

    private void Start()
    {
        ComputeGridPositionList();
    }

    private void ComputeGridPositionList()
    {
        gridPositionList = Helpers.GetGridPositionsCoveredByObject(
            GetComponent<BoxCollider>().bounds,
            LevelGrid.Instance.GetCellSize()
        );
    }

    public override void Damage()
    {
        DestroyObject();
    }
}
