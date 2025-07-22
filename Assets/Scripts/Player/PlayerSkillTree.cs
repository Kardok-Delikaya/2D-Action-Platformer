using UnityEngine;

public class PlayerSkillTree : MonoBehaviour
{
    [Header("Character Level")]
    public int characterLevel=0;

    [Header("Armor Level")]
    public int armorLevel=0;
    //Level 1:Wall Jump
    //Level 2:+1 Potion
    //Level 3:+10 Physical Defence
    //Level 4:+1 Potion
    //Level 5:+10 Magic Defence

    [Header("Shield Upgrades")]
    public bool canDoParry;
    public bool moreParryLength;
    public bool lessDamageWhileBlocking;
    public bool stunningAfterBlack;
    public bool damageBoostAfterParry; //+%10
    public bool stunnedEnemiesGetsMoreDamage; //+%30

    [Header("Weapon Upgrades")]
    public bool canDoCounterAttack;
    public bool canDoKickAttackAfterRoll;
    public bool canMakeFlyingEnemiesFallToGroundWhileAttackingInAir;
    public bool deadEnemiesDropsMoreLoot;

    [Header("Magic Upgrades")]
    public bool afterParryPushEnemies;
    public bool magicStunnsEnemies;
    public bool magicJumpsFromEnemyToEnemy;
    public bool enemiesThatKilledByMagicDropsManaOrbs;
    public bool afterMagicAddMagicDamageToWeapon;
    public bool afterMagicFirstBlockSendsDamageBackToEnemy;

    [Header("Ultimate Upgrades")]
    public bool weaponSendsLightRays;
    public bool takingDamageWhileBlockingSendEnemiesDamageRays;
    public bool rollingDealDamagesToEnemies;
    public bool healWhenDealLightDamageAndRegainManaWhenHealed; // heal 1/3 of light damage and get mana 1/3 of heal
    public bool dealMagicDamageToBothWaysAndLonger;
}
