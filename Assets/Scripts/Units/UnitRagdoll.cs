using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollRootBone;


    public void Setup(Transform originalRootBone, Vector3 explosionPosition)
    {
        MatchAllChildTransform(originalRootBone, ragdollRootBone);
        ApplyExplosionToRagdoll(ragdollRootBone, 500f, explosionPosition, 10f);
    }

    private void MatchAllChildTransform(Transform root, Transform clone)
    {
        foreach (Transform child in root)
        {
            Transform cloneChild = clone.Find(child.name);
            if (cloneChild)
            {
                cloneChild.position = child.position;
                cloneChild.rotation = child.rotation;

                MatchAllChildTransform(child, cloneChild);
            }
        }
    }

    private void ApplyExplosionToRagdoll(Transform root, float explosionForce, Vector3 explosionposition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionposition, explosionRange);

            }

            ApplyExplosionToRagdoll(child, explosionForce, explosionposition, explosionRange);
        }
    }
}
