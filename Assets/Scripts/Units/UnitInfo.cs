using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfo : MonoBehaviour
{
    [SerializeField] private string unitName = "UnitName";
    [SerializeField] private bool isEnemy = false;
    [SerializeField] private int maxInteractDistance = 1;
    [SerializeField] private int maxMoveDistance = 6;
    [SerializeField] private int maxShootDistance = 7;
    [SerializeField] private int maxThrowDistance = 5;
    [SerializeField] private int maxSwordDistance = 1;

    [SerializeField] private int actionPointsMax = 2;

    [SerializeField] private int fightDamage = 70;
    [SerializeField] private int shootDamage = 40;

    public string GetUnitName() => unitName;
    public bool IsEnemy() => isEnemy;
    public int GetMaxInteractDistance() => maxInteractDistance;
    public int GetMaxMoveDistance() => maxMoveDistance;
    public int GetMaxShootDistance() => maxShootDistance;
    public int GetMaxThrowDistance() => maxThrowDistance;
    public int GetMaxSwordDistance() => maxSwordDistance;
    public int GetActionPointsMax() => actionPointsMax;
    public int GetFightDamage() => fightDamage;
    public int GetShootDamage() => shootDamage;

}
