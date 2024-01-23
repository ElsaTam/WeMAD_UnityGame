using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spin : 0
// Interact : 0
// Move to get closer : 0 --> 49
// Move to get shoot  : 50 --> 50 + possibleTargets
// Attack weakest unit : 100 --> 200
// Grenade : 70 * number of targets
public class EnemyAIAction
{
    public GridPosition gridPosition;
    public int actionValue;

    public int GetSpinActionValue() => 0;
    public int GetInteractActionValue() => 0;
    public int GetMoveToGetCloserActionValueMax() => 49;
    public int GetMoveToGetShootActionValueMin() => 50;
    public int GetAttackActionValueMin() => 100;
    public int GetGrenadeActionValueFactor() => 70;
}
