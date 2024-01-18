using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isOpen = false;

    private Animator animator;
    private List<GridPosition> gridPositionList;
    private Action onInteractionComplete;
    private bool isActive;
    private float timer;
    

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

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

        if (isOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
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



    public void Interact(Action onInteractionComplete)
    {
        this.onInteractionComplete = onInteractionComplete;
        isActive = true;
        timer = 0.5f;


        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        animator.SetBool("IsOpen", isOpen);
        UpdatePathFinding();
    }

    private void CloseDoor()
    {
        isOpen = false;
        animator.SetBool("IsOpen", isOpen);
        UpdatePathFinding();
    }

    private void UpdatePathFinding()
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            PathFinding.Instance.SetIsWalkableGridPosition(gridPosition, isOpen);
        }
    }
}
