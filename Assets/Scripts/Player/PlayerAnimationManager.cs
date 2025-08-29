using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
    private Animator anim;
    private PlayerManager player;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        player = GetComponent<PlayerManager>();
    }

    private void Update()
    {
        anim.SetFloat(Move, Mathf.Abs(GetComponent<Rigidbody2D>().linearVelocityX));
        anim.SetBool(IsGrounded, player.playerMovement.IsGrounded());
    }

    public void PlayAnimation(string animationName, bool isInteracting = true)
    {
        player.isInteracting = isInteracting;
        anim.Play(animationName);
    }
}
