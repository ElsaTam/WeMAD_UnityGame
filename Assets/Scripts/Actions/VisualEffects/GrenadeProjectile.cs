using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    public static event EventHandler OnAnyGrenadeExploded;

    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform grenadeExplodeVfxPrefab;
    [SerializeField] private AnimationCurve arcYAnimationCurve;

    private Vector3 targetPosition;
    private Action<Vector3> onGrenadeBehaviourComplete;
    private float maxHeightRatio = 0.25f;
    private float heightAtTargetExpected = 0.5f;
    private float totalDistance;
    private Vector3 positionXZ;

    private void Update()
    {
        Vector3 moveDir = (targetPosition - positionXZ).normalized;
        float grenadeSpeed = 15f;
        positionXZ += grenadeSpeed * Time.deltaTime * moveDir;

        float distance = Vector3.Distance(targetPosition, positionXZ);
        float distanceNormalized = 1f - distance / totalDistance;
        if (distanceNormalized > 0f)
        {
            trailRenderer.gameObject.SetActive(true);
        }

        float maxHeight = totalDistance * maxHeightRatio;
        float heightAtTargetEvaluated = arcYAnimationCurve.Evaluate(1f) * maxHeight;
        float heightOffset = heightAtTargetExpected - heightAtTargetEvaluated;
        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight + heightOffset;
        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);

        float reachedTargetDistance = 0.2f;
        if (Vector3.Distance(transform.position, targetPosition) < reachedTargetDistance)
        {
            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);

            Instantiate(grenadeExplodeVfxPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);
            trailRenderer.transform.parent = null;
            Destroy(gameObject);

            onGrenadeBehaviourComplete(targetPosition);
        }
    }

    public void Setup(GridPosition targetGridPosition, Action<Vector3> onGrenadeBehaviourComplete)
    {
        trailRenderer.gameObject.SetActive(false);
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition) + Vector3.up * heightAtTargetExpected;
        positionXZ = Vector3.ProjectOnPlane(transform.position, Vector3.up);
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
        this.onGrenadeBehaviourComplete = onGrenadeBehaviourComplete;
    }
}
