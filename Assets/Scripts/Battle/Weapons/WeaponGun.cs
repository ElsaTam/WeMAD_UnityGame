using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGun : IWeapon
{
    IWeapon.WeaponTypes IWeapon.GetType() => IWeapon.WeaponTypes.Gun;
    public int GetAccuracy() => 90;
    public int GetCritical() => 0;
    public int GetMight() => 2;
    public int GetRange() => 6;
    public int GetWeight() => 3;
}
