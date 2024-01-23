using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://fireemblem.fandom.com/wiki/Battle_Formulas
public class BattleSystem : MonoBehaviour
{

    public static BattleSystem Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one BattleSystem. " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private int GetSupportBonus(Unit unit)
    {
        return 0;
    }


    public void PerformRound(Unit attackingUnit, Unit targetUnit)
    {
        // Attacker first attack
        if (PerformAttack(attackingUnit, targetUnit)) return;

        // Defender first attack
        bool targetCanAttack = targetUnit.GetAttackAction().GetValidActionGridPositionList().Contains(attackingUnit.GetGridPosition());
        if (targetCanAttack)
        {
            if (PerformAttack(targetUnit, attackingUnit)) return;
        }

        // Attacker second attack
        if (AttacksTwice(attackingUnit, targetUnit))
        {
            if (PerformAttack(attackingUnit, targetUnit)) return;
        }

        // Defender second attack
        if (targetCanAttack)
        {
            if (AttacksTwice(targetUnit, attackingUnit))
            {
                if (PerformAttack(targetUnit, attackingUnit)) return;
            }
        }
    }

    public bool PerformAttack(Unit attackingUnit, Unit targetUnit)
    {
        if (Random.Range(0, 100) < GetAccuracy(attackingUnit, targetUnit))
        {
            int damage;
            if (Random.Range(0, 100) < GetCriticalChance(attackingUnit, targetUnit))
            {
                damage = GetCriticalDamage(attackingUnit, targetUnit);
            }
            else
            {
                damage = GetDamage(attackingUnit, targetUnit);
            }
            Debug.Log(damage);

            targetUnit.Damage(damage, attackingUnit.GetAttackTargetPosition());

            if (targetUnit.GetHealth() <= damage)
            {
                return true;
            }
        }
        return false;
    }

    // Attack Speed is the value used to determine whether or not a unit within combat will perform a double attack against its opposing foe.
    private int GetAttackSpeed(Unit unit)
    {
        UnitInfo unitInfo = unit.GetUnitInfo();
        IWeapon weapon = unit.GetWeapon();
        if (weapon.GetWeight() <= unitInfo.GetStrength())
            return unitInfo.GetSpeed();
        else
            return unitInfo.GetSpeed() - weapon.GetWeight() + unitInfo.GetStrength();
    }

    // A unit will attack twice in a battle if its attack speed is at least a certain amount higher than the opposing unit's attack speed.
    public bool AttacksTwice(Unit attackingUnit, Unit targetUnit)
    {
        return GetAttackSpeed(attackingUnit) >= GetAttackSpeed(targetUnit) + 4;
    }



    // Hit Rate is the base chance a character has of hitting an enemy.
    public int GetHitRate(Unit unit)
    {
        UnitInfo unitInfo = unit.GetUnitInfo();
        IWeapon weapon = unit.GetWeapon();
        int skl = unitInfo.GetSkill();
        int lck = unitInfo.GetLuck();
        int bonusClass = 0;
        switch (unitInfo.GetClass())
        {
            case UnitInfo.UnitClass.Sniper:
                bonusClass = 5;
                break;
            case UnitInfo.UnitClass.Fighter:
                bonusClass = 10;
                break;
        }
        int wRank = unitInfo.GetWeaponRank(weapon);
        int hit = weapon.GetAccuracy();
        int supp = GetSupportBonus(unit);
        return skl + Mathf.RoundToInt(lck * 0.5f) + bonusClass + wRank + hit + supp;
    }

    // Avoid (or Evade) is a measure of how well a unit can avoid being hit by an enemy's attack. 
    public int GetAvoid(Unit unit)
    {
        UnitInfo unitInfo = unit.GetUnitInfo();
        int AS = GetAttackSpeed(unit);
        int lck = unitInfo.GetLuck();
        int terr = 0;
        int supp = GetSupportBonus(unit);
        return AS + Mathf.RoundToInt(lck * 0.5f) + terr + supp;
    }

    // Accuracy is used in battle to determine how likely a character is to hit another character, using the difference between the attacker's hit rate and the defender's evade.
    private int GetAccuracy(Unit attackingUnit, Unit targetUnit)
    {
        return GetHitRate(attackingUnit) - GetAvoid(targetUnit);
    }



    // Attack is used to calculate how much someone's attack could deal under optimum conditions (excluding abilities and criticals which are calculated elsewhere.) 
    public int GetAttackPower(Unit unit)
    {
        UnitInfo unitInfo = unit.GetUnitInfo();
        IWeapon weapon = unit.GetWeapon();
        int basePower = unitInfo.GetStrength();
        if (weapon.GetType() == IWeapon.WeaponTypes.Artifact) basePower = unitInfo.GetMagic();
        return basePower + weapon.GetMight() * unitInfo.GetWeaponRank(weapon);
    }

    // Defense power is a measure of the total damage a character can negate from enemy attacks.
    public int GetDefensePower(Unit unit, IWeapon.WeaponTypes weaponType)
    {
        UnitInfo unitInfo = unit.GetUnitInfo();
        int basePower = unitInfo.GetDefense();
        if (weaponType == IWeapon.WeaponTypes.Artifact) basePower = unitInfo.GetResistance();
        int terr = 0;
        int supp = GetSupportBonus(unit);
        return basePower + terr + supp;
    }

    // Damage is the amount of health an attack takes away from a defending unit if the attack hits.
    private int GetDamage(Unit attackingUnit, Unit targetUnit)
    {
        return Mathf.Max(0, GetAttackPower(attackingUnit) - GetDefensePower(targetUnit, attackingUnit.GetWeapon().GetType()));
    }



    // When attacking, characters often have the chance to strike a critical hit. This will in general do a vastly higher amount of damage than a regular hit. 
    private int GetCriticalDamage(Unit attackingUnit, Unit targetUnit)
    {
        return GetDamage(attackingUnit, targetUnit) * 3;
    }

    // A character's critical rate is the likelihood of their managing to perform a critical hit against a stationary target. 
    public int GetCriticalRate(Unit unit)
    {
        UnitInfo unitInfo = unit.GetUnitInfo();
        IWeapon weapon = unit.GetWeapon();
        int bonusClass = unitInfo.GetClass() == UnitInfo.UnitClass.Sniper ? 5 : 0;
        int supp = GetSupportBonus(unit);
        return weapon.GetCritical() + Mathf.FloorToInt(unitInfo.GetSkill() * 0.5f) + bonusClass + supp;
    }

    // Critical Evade is to Critical Rate as Evade is to Accuracy. It is a measurement of the chance a character has to detect an approaching critical hit and react accordingly; the amount by which an attacker's critical rate is reduced. 
    public int GetCriticalAvoid(Unit unit)
    {
        UnitInfo unitInfo = unit.GetUnitInfo();
        return unitInfo.GetLuck() + GetSupportBonus(unit);
    }

    // Critical chance is the percentage used in battle to determine the chance of a character landing a critical hit.
    private int GetCriticalChance(Unit attackingUnit, Unit targetUnit)
    {
        return Mathf.Clamp(GetCriticalRate(attackingUnit) - GetCriticalAvoid(targetUnit), 0, 100);
    }

}
