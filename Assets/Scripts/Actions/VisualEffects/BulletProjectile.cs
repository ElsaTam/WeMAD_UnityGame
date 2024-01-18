using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletHitVfxPrefab;

    private Vector3 targetPosition;

    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void Update()
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        
        float moveSpeed = 200f;
        transform.position += moveSpeed * Time.deltaTime * moveDirection;

        // Check if bullet has passed targetPoint
        Vector3 moveDirectionAfter = (targetPosition - transform.position).normalized;
        if (Vector3.Dot(moveDirection, moveDirectionAfter) < 0)
        {
            transform.position = targetPosition;
            trailRenderer.transform.parent = null;
            Destroy(gameObject);

            Instantiate(bulletHitVfxPrefab, targetPosition, Quaternion.identity);
        }

    }
}
