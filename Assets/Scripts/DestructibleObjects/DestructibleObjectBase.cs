using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DestructibleObjectBase : MonoBehaviour
{
    public static event EventHandler OnAnyDestroyed;

    [SerializeField] protected Transform objectDestroyedPrefab;
    protected List<GridPosition> gridPositionList;


    public List<GridPosition> GetGridPositionList() => gridPositionList;

    public abstract void Damage();

    protected void DestroyObject()
    {
        Transform objectDestroyedTransform = Instantiate(objectDestroyedPrefab, transform.position, transform.rotation);
        ApplyExplosionToObjectChildren(objectDestroyedTransform, 150f, transform.position, 10f);

        Destroy(gameObject);
        OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
    }

    private void ApplyExplosionToObjectChildren(Transform root, float explosionForce, Vector3 explosionposition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionposition, explosionRange);

            }

            ApplyExplosionToObjectChildren(child, explosionForce, explosionposition, explosionRange);
        }
    }
}
