using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    public enum WeaponTypes
    {
        Hands,
        Sword,
        Gun,
        Sniper,
        Artifact
    }

    WeaponTypes GetType();
    int GetWeight();
    int GetAccuracy();
    int GetRange();
    int GetMight();
    int GetCritical();
}
