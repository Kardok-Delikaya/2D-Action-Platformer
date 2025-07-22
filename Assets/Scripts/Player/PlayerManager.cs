using UnityEngine;

[RequireComponent(typeof(PlayerSkillTree))]
[RequireComponent(typeof(PlayerMovementManager))]
[RequireComponent(typeof(PlayerCombatManager))]
[RequireComponent(typeof(PlayerAnimationManager))]
public class PlayerManager : MonoBehaviour
{
    public bool isDead;
    public bool isInteracting;

    public UIManager uiManager;
    public PlayerSkillTree skillTree;
    public PlayerMovementManager playerMovement;
    public PlayerCombatManager playerCombat;
    public PlayerAnimationManager playerAnimation;

    private void Awake()
    {
        uiManager = FindAnyObjectByType<UIManager>();
        skillTree = GetComponent<PlayerSkillTree>(); 
        playerMovement = GetComponent<PlayerMovementManager>();
        playerCombat = GetComponent<PlayerCombatManager>();
        playerAnimation = GetComponent<PlayerAnimationManager>();
    }
}
