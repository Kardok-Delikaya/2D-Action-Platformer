using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    Animator anim;
    PlayerManager player;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        player = GetComponent<PlayerManager>();
    }

    private void Update()
    {
        anim.SetFloat("Move", Mathf.Abs(GetComponent<Rigidbody2D>().linearVelocityX));
        anim.SetBool("isGrounded", player.playerMovement.IsGrounded());
    }

    public void PlayAnimation(string animationName, bool isInteracting = true)
    {
        player.isInteracting = isInteracting;
        anim.Play(animationName);
    }
}
