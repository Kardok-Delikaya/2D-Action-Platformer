using UnityEngine;

[RequireComponent(typeof(PlayerSkillTree))]
[RequireComponent(typeof(PlayerMovementManager))]
[RequireComponent(typeof(PlayerCombatManager))]
[RequireComponent(typeof(PlayerAnimationManager))]
public class PlayerManager : MonoBehaviour
{
    public bool isDead;
    public bool isInteracting;

    public UIManager uiManager { get; private set; }
    public PlayerSkillTree skillTree { get; private set; }
    public PlayerMovementManager playerMovement { get; private set; }
    public PlayerCombatManager playerCombat { get; private set; }
    public PlayerAnimationManager playerAnimation { get; private set; }

    private void Awake()
    {
        uiManager = FindAnyObjectByType<UIManager>();
        skillTree = GetComponent<PlayerSkillTree>(); 
        playerMovement = GetComponent<PlayerMovementManager>();
        playerCombat = GetComponent<PlayerCombatManager>();
        playerAnimation = GetComponent<PlayerAnimationManager>();
    }
}
