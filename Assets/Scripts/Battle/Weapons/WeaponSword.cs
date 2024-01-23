using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class WeaponSword : IWeapon
{
    IWeapon.WeaponTypes IWeapon.GetType() => IWeapon.WeaponTypes.Sword;
    public int GetAccuracy() => 100;
    public int GetCritical() => 0;
    public int GetMight() => 3;
    public int GetRange() => 1;
    public int GetWeight() => 6;
}
