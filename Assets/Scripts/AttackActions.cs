using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Attack Actions")]
public class AttackActions : ScriptableObject
{
    [Header("Animation")]
    public string attackAnimation="Light_Attack_01";

    [Header("Damage Values")]
    public float physicalDamage = 3;
    public float magicDamage = 0;
    public float holyDamage = 0;
    public float stunDuration = 0;
    public float pushForce = 0;
}