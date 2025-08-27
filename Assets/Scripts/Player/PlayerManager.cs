using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerSkillTree))]
[RequireComponent(typeof(PlayerMovementManager))]
[RequireComponent(typeof(PlayerCombatManager))]
[RequireComponent(typeof(PlayerAnimationManager))]
public class PlayerManager : MonoBehaviour
{
    public bool isDead;
    public bool isInteracting;

    public PlayerSkillTree skillTree { get; private set; }
    public PlayerMovementManager playerMovement { get; private set; }
    public PlayerCombatManager playerCombat { get; private set; }
    public PlayerAnimationManager playerAnimation { get; private set; }

    private void Awake()
    {
        skillTree = GetComponent<PlayerSkillTree>(); 
        playerMovement = GetComponent<PlayerMovementManager>();
        playerCombat = GetComponent<PlayerCombatManager>();
        playerAnimation = GetComponent<PlayerAnimationManager>();
    }
}