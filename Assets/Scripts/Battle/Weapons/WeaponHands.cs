using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHands : IWeapon
{
    IWeapon.WeaponTypes IWeapon.GetType() => IWeapon.WeaponTypes.Hands;
    public int GetAccuracy() => 100;
    public int GetCritical() => 0;
    public int GetMight() => 1;
    public int GetRange() => 1;
    public int GetWeight() => 0;
}
