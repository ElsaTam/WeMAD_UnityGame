using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSphere : MonoBehaviour, IInteractable
{
    
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private MeshRenderer meshRenderer;

    private bool isGreen;
    private List<GridPosition> gridPositionList;
    private Action onInteractionComplete;
    private bool isActive;
    private float timer;
    
    private void Start()
    {
        gridPositionList = Helpers.GetGridPositionsCoveredByObject(
            GetComponent<BoxCollider>().bounds,
            LevelGrid.Instance.GetCellSize()
        );
        foreach (GridPosition gridPosition in gridPositionList)
        {
            LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
        }

        SetColorGreen();
    }

    private void Update()
    {
        if (! isActive) return;

        timer += -Time.deltaTime;

        if (timer < 0f)
        {
            isActive = false;
            onInteractionComplete();
        }
    }



    private void SetColorGreen()
    {
        isGreen = true;
        meshRenderer.material = greenMaterial;
    }

    private void SetColorRed()
    {
        isGreen = false;
        meshRenderer.material = redMaterial;
    }

    public void Interact(Action onInteractionComplete)
    {
        this.onInteractionComplete = onInteractionComplete;
        isActive = true;
        timer = 0.5f;

        if (isGreen) SetColorRed();
        else SetColorGreen();
    }
}
