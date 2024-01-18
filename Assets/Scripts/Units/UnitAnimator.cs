using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    
    [SerializeField] private Animator animator;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform grenadeProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;
    [SerializeField] private Transform rifleTransform;
    [SerializeField] private Transform swordTransform;

    private void Awake()
    {
        TryGetComponent<MoveAction>(out MoveAction moveAction);
        if (moveAction)
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }

        TryGetComponent<ShootAction>(out ShootAction shootAction);
        if (shootAction)
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }

        TryGetComponent<GrenadeAction>(out GrenadeAction grenadeAction);
        if (grenadeAction)
        {
            grenadeAction.OnThrow += GrenadeAction_OnThrow;
        }

        TryGetComponent<SwordAction>(out SwordAction swordAction);
        if (swordAction)
        {
            swordAction.OnSwordActionStarted += SwordAction_OnSwordActionStarted;
            swordAction.OnSwordActionCompleted += SwordAction_OnSwordActionCompleted;
        }
    }

    private void Start()
    {
        EquipRifle();
    }



    private void EquipRifle()
    {
        rifleTransform.gameObject.SetActive(true);
        swordTransform.gameObject.SetActive(false);
    }

    private void EquipSword()
    {
        rifleTransform.gameObject.SetActive(false);
        swordTransform.gameObject.SetActive(true);
    }


    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool("IsWalking", true);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool("IsWalking", false);
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        animator.SetTrigger("Shoot");

        Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);
        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

        bulletProjectile.Setup(e.targetUnit.GetAttackTargetPosition());
    }

    private void GrenadeAction_OnThrow(object sender, GrenadeAction.OnThrowEventArgs e)
    {
        //animator.SetTrigger("Throw");

        Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, e.shootingUnit.GetWorldPosition(), Quaternion.identity);
        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();

        grenadeProjectile.Setup(e.targetGridPosition, ((GrenadeAction)sender).OnGrenadeBehaviourComplete);
    }

    private void SwordAction_OnSwordActionStarted(object sender, EventArgs e)
    {
        EquipSword();
        animator.SetTrigger("SwordSlash");
    }

    private void SwordAction_OnSwordActionCompleted(object sender, EventArgs e)
    {
        EquipRifle();
    }
}
