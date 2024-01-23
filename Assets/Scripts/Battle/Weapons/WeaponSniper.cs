using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSniper : IWeapon
{
    IWeapon.WeaponTypes IWeapon.GetType() => IWeapon.WeaponTypes.Sniper;
    public int GetAccuracy() => 100;
    public int GetCritical() => 0;
    public int GetMight() => 5;
    public int GetRange() => 12;
    public int GetWeight() => 5;
}
