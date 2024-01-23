using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponArtifact : IWeapon
{
    IWeapon.WeaponTypes IWeapon.GetType() => IWeapon.WeaponTypes.Artifact;
    public int GetAccuracy() => 100;
    public int GetCritical() => 0;
    public int GetMight() => 4;
    public int GetRange() => 6;
    public int GetWeight() => 1;
}
