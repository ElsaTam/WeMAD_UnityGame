using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfo : MonoBehaviour
{
    public enum UnitClass
    {
        Brawler, // bare hands
        Fighter, // sword or other weapon
        Pistolero, // flingue
        Sniper, // sniper
        Mage
    }

    public struct WeaponLevels
    {
        public int hands;
        public int sword;
        public int gun;
        public int sniper;
        public int artifact;
    }

    public struct Stats
    {
        public int hp;
        public int strength;
        public int magic;
        public int skill;
        public int speed;
        public int luck;
        public int defense;
        public int resistance;
        public int movement;
        public WeaponLevels weaponLevels;
    }

    [SerializeField] private string unitName = "UnitName";
    [SerializeField] private bool isEnemy = false;
    [SerializeField] private UnitClass unitClass;
    [SerializeField] private int maxThrowDistance = 5;

    [SerializeField] private int actionPointsMax = 1;

    private Stats baseStats;
    private Stats maxStats;
    private Stats growthStatsMin;
    private Stats growthStatsMax;
    private Stats currentStats;


    private void Awake()
    {
        switch (unitClass)
        {
            case UnitClass.Brawler:
                baseStats =      new Stats{hp=20, strength=5, magic=0, skill=1, speed=7, luck=0, defense=6, resistance=0, movement=6,
                                           weaponLevels = new WeaponLevels{hands=1, sword=0, gun=0, sniper=0, artifact=0}};
                maxStats =       new Stats{hp=60, strength=30, magic=20, skill=26, speed=24, luck=30, defense=30, resistance=25, movement=6,
                                           weaponLevels = new WeaponLevels{hands=5, sword=0, gun=0, sniper=0, artifact=0}};
                growthStatsMin = new Stats{hp=80, strength=40, magic=0, skill=15, speed=10, luck=0, defense=20, resistance=0, movement=0,
                                           weaponLevels = new WeaponLevels{hands=0, sword=0, gun=0, sniper=0, artifact=0}};
                growthStatsMax = new Stats{hp=90, strength=50, magic=0, skill=30, speed=20, luck=0, defense=50, resistance=20, movement=0,
                                           weaponLevels = new WeaponLevels{hands=0, sword=0, gun=0, sniper=0, artifact=0}};
                break;
            case UnitClass.Fighter:
                baseStats =      new Stats{hp=18, strength=4, magic=0, skill=9, speed=11, luck=0, defense=5, resistance=0, movement=7,
                                           weaponLevels = new WeaponLevels{hands=0, sword=1, gun=0, sniper=0, artifact=0}};
                maxStats =       new Stats{hp=60, strength=26, magic=20, skill=30, speed=30, luck=30, defense=24, resistance=23, movement=7,
                                           weaponLevels = new WeaponLevels{hands=0, sword=5, gun=0, sniper=0, artifact=0}};
                growthStatsMin = new Stats{hp=50, strength=20, magic=0, skill=20, speed=25, luck=0, defense=15, resistance=0, movement=0,
                                           weaponLevels = new WeaponLevels{hands=0, sword=0, gun=0, sniper=0, artifact=0}};
                growthStatsMax = new Stats{hp=50, strength=20, magic=0, skill=30, speed=25, luck=0, defense=30, resistance=5, movement=0,
                                           weaponLevels = new WeaponLevels{hands=0, sword=0, gun=0, sniper=0, artifact=0}};
                break;
            case UnitClass.Pistolero:
                baseStats =      new Stats{hp=16, strength=3, magic=0, skill=3, speed=5, luck=0, defense=4, resistance=0, movement=6,
                                           weaponLevels = new WeaponLevels{hands=0, sword=0, gun=1, sniper=0, artifact=0}};
                maxStats =       new Stats{hp=60, strength=24, magic=20, skill=27, speed=30, luck=30, defense=23, resistance=20, movement=6,
                                           weaponLevels = new WeaponLevels{hands=5, sword=0, gun=0, sniper=0, artifact=0}};
                growthStatsMin = new Stats{hp=40, strength=20, magic=0, skill=20, speed=30, luck=0, defense=10, resistance=0, movement=0,
                                           weaponLevels = new WeaponLevels{hands=0, sword=0, gun=0, sniper=0, artifact=0}};
                growthStatsMax = new Stats{hp=80, strength=30, magic=0, skill=30, speed=30, luck=0, defense=20, resistance=0, movement=0,
                                           weaponLevels = new WeaponLevels{hands=0, sword=0, gun=0, sniper=0, artifact=0}};
                break;
            case UnitClass.Sniper:
                baseStats =      new Stats{hp=14, strength=2, magic=0, skill=4, speed=2, luck=0, defense=4, resistance=0, movement=4,
                                           weaponLevels = new WeaponLevels{hands=0, sword=0, gun=0, sniper=1, artifact=0}};
                maxStats =       new Stats{hp=60, strength=24, magic=20, skill=27, speed=24, luck=30, defense=23, resistance=20, movement=4,
                                           weaponLevels = new WeaponLevels{hands=0, sword=0, gun=0, sniper=5, artifact=0}};
                growthStatsMin = new Stats{hp=20, strength=10, magic=0, skill=20, speed=10, luck=0, defense=10, resistance=0, movement=0,
                                           weaponLevels = new WeaponLevels{hands=0, sword=0, gun=0, sniper=0, artifact=0}};
                growthStatsMax = new Stats{hp=50, strength=20, magic=0, skill=40, speed=20, luck=0, defense=20, resistance=0, movement=0,
                                           weaponLevels = new WeaponLevels{hands=0, sword=0, gun=0, sniper=0, artifact=0}};
                break;
            case UnitClass.Mage:
                baseStats =      new Stats{hp=14, strength=1, magic=3, skill=2, speed=4, luck=0, defense=2, resistance=3, movement=5,
                                           weaponLevels = new WeaponLevels{hands=0, sword=0, gun=0, sniper=0, artifact=1}};
                maxStats =       new Stats{hp=60, strength=20, magic=30, skill=28, speed=25, luck=30, defense=20, resistance=30, movement=5,
                                           weaponLevels = new WeaponLevels{hands=0, sword=0, gun=0, sniper=0, artifact=5}};
                growthStatsMin = new Stats{hp=20, strength=0, magic=30, skill=20, speed=15, luck=0, defense=0, resistance=20, movement=0,
                                           weaponLevels = new WeaponLevels{hands=0, sword=0, gun=0, sniper=0, artifact=0}};
                growthStatsMax = growthStatsMin;
                break;
        }
        currentStats = baseStats;
    }

    public string GetUnitName() => unitName;
    public bool IsEnemy() => isEnemy;
    public UnitClass GetClass() => unitClass;
    public int GetActionPointsMax() => actionPointsMax;
    public int GetMaxThrowDistance() => maxThrowDistance;

    public int GetHP() => currentStats.hp;
    public int GetStrength() => currentStats.strength;
    public int GetMagic() => currentStats.magic;
    public int GetSkill() => currentStats.skill;
    public int GetSpeed() => currentStats.speed;
    public int GetLuck() => currentStats.luck;
    public int GetDefense() => currentStats.defense;
    public int GetResistance() => currentStats.resistance;
    public int GetMovement() => currentStats.movement;

    public int GetWeaponRank(IWeapon weapon)
    {
        switch (weapon.GetType())
        {
            case IWeapon.WeaponTypes.Hands:
                return currentStats.weaponLevels.hands;
            case IWeapon.WeaponTypes.Sword:
                return currentStats.weaponLevels.sword;
            case IWeapon.WeaponTypes.Gun:
                return currentStats.weaponLevels.gun;
            case IWeapon.WeaponTypes.Sniper:
                return currentStats.weaponLevels.sniper;
            case IWeapon.WeaponTypes.Artifact:
                return currentStats.weaponLevels.artifact;
        }
        return 0;
    }

    

}
