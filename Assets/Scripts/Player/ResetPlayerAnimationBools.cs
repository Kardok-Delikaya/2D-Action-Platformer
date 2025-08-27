using UnityEngine;

public class ResetPlayerAnimationBools : StateMachineBehaviour
{
    private PlayerManager player;

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerManager>();
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!FindAnyObjectByType<DialogueManager>().isDialogueActive)
            player.isInteracting = false;

        player.playerCombat.isInvulnerable = false;
        player.playerCombat.isBlocking = false;
        //player.playerCombat.isStunned = false;
        player.playerCombat.isAttacking = false;
    }
}