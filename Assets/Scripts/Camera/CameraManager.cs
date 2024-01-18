using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject actionCameraGameObject;
    [SerializeField] private bool useActionCamera = false;

    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;

        HideActionCamera();
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        if (! useActionCamera) return;

        switch(sender)
        {
            case ShootAction shootAction:
                Unit shooterUnit = shootAction.GetUnit();
                Unit targetUnit = shootAction.GetTargetUnit();
                Vector3 cameraPosition = ComputeCameraPosition(shooterUnit, targetUnit);
                actionCameraGameObject.transform.position = cameraPosition;
                actionCameraGameObject.transform.LookAt(targetUnit.GetAttackTargetPosition());
                ShowActionCamera();
                break;
        }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        if (! useActionCamera) return;

        switch(sender)
        {
            case ShootAction:
                HideActionCamera();
                break;
        }
    }

    private Vector3 ComputeCameraPosition(Unit activeUnit, Unit targetUnit)
    {
        Vector3 cameraCharacterHeight = Vector3.up * 1.7f;
        Vector3 shootDirection = (targetUnit.GetWorldPosition() - activeUnit.GetWorldPosition()).normalized;
        float shoulderOffsetAmount = 0.5f;
        Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDirection * shoulderOffsetAmount;
        Vector3 cameraPosition = activeUnit.GetWorldPosition() + cameraCharacterHeight + shoulderOffset - shootDirection;
        return cameraPosition;
    }

    private void ShowActionCamera()
    {
        actionCameraGameObject.SetActive(true);
    }
    
    private void HideActionCamera()
    {
        actionCameraGameObject.SetActive(false);
    }
}
