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
    //Level 2: More Parry Length or Less Damage While Blocking
    //Level 3: Can Stun Attack After Parry
    //Level 4: Damage Boost After Parry or Enemies Stunned By Parry Gets More Damage

    //Weapon Upgrades
    //Level 1: Can Counter Attack After Parry
    //Level 2: Can Kick Attack After Roll or Can Throw Flying Enemies to Ground While Heavy Attacking in Air
    //Level 3: Killed Enemies Drops More Loot
    //Level 4: *Null*

    //Magic Upgrades
    //Level 1: After Parry More Length and Damage
    //Level 2: Magic Stuns Enemies For Longer or Magic Jumps From Enemy To Enemy 2 times with less damage
    //Level 3: Enemies Killed By Magic Attack Drops little Mana Orbs
    //Level 4: After Magic Attack Weapon Does Extra Magic Damage or First Block Sends Damage Back to Enemy as Magic Damage 
    
    //Ultimate Upgrades
    //Weapon Attacks Sends Light Rays
    //More Defence while Blocking
    //Rolling Deals Damages to Enemies
    //Get heals 1/3 of Light Damage Dealted, Regain mana 1/3 of heals
    //Magic attack goes to both way instead of just forward
}
