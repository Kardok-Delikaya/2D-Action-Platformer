using ActionPlatformer;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMovementManager : MonoBehaviour
{
    Rigidbody2D rb;
    PlayerManager player;
    bool beingPushed;

    [Header("Movement")]
    public float moveSpeed = 10f;
    public int horizontalDirection = 1;
    float horizontalMovement;
    float verticalMovement;

    [Header("Rolling")]
    public float rollSpeed = 20f;
    public float rollDuration = 0.25f;
    public float rollCoolDown = 1f;
    public bool isRolling;
    bool canRoll = true;

    [Header("Jump")]
    public float jumpPower = 20f;

    [Header("Ground Check")]
    public Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
    public Vector2 wallCheckSize = new Vector2(0.03f, 0.49f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float baseGravity = 3f;
    public float maxFallSpeed = 16f;
    public float fallGravityMult = 4f;

    [Header("Wall Movement")]
    public float wallSlideSpeed = 2f;
    bool isWallSliding;
    bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.5f;
    public Vector2 wallJumpPower = new Vector2(10f, 20f);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<PlayerManager>();
    }

    void Update()
    {
        ProcessGravity();

        if (isRolling || player.playerCombat.isStunned || player.isDead || beingPushed)
            return;


        //ProcessWallSlide();
        //ProcessWallJump();

        if (!isWallJumping)
        {
            if (player.playerCombat.isAttacking)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
            }
            else
            {
                rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocityY);

                if (horizontalDirection==1 && horizontalMovement < 0 || horizontalDirection == -1 && horizontalMovement > 0)
                {
                    Flip();
                }
            }
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
        verticalMovement = context.ReadValue<Vector2>().y;
    }

    public void Roll(InputAction.CallbackContext context)
    {
        if (player.isInteracting)
            return;

        if (context.performed && canRoll && IsGrounded())
        {
            StartCoroutine(RollCoroutine());
            StartCoroutine(FindAnyObjectByType<CameraShakeEffect>().ShakeCameraCorutine(1, .05f));
        }
    }

    private IEnumerator RollCoroutine()
    {
        canRoll = false;
        isRolling = true;
        player.playerAnimation.PlayAnimation("Roll");


        rb.linearVelocity = new Vector2(horizontalDirection * rollSpeed, rb.linearVelocityY);

        yield return new WaitForSeconds(rollDuration);

        rb.linearVelocityX /= 2;
        isRolling = false;

        yield return new WaitForSeconds(rollCoolDown);
        canRoll = true;
    }

    public IEnumerator PushBackCoroutine(int pushForce)
    {
        beingPushed = true;
        rb.linearVelocityX = pushForce;

        yield return new WaitForSeconds(.2f);

        rb.linearVelocityX = 0;
        beingPushed =false;
    }

    public IEnumerator StunCoroutine(float stunDuration)
    {
        player.playerCombat.isStunned = true;

        yield return new WaitForSeconds(stunDuration);

        player.playerCombat.isStunned = false;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (player.isInteracting)
            return;

        if (context.performed)
        {
            if (IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpPower);
                player.playerAnimation.PlayAnimation("Jump", false);
                StartCoroutine(FindAnyObjectByType<CameraShakeEffect>().ShakeCameraCorutine(1f, .1f));
            }
        }
        else if (context.canceled && rb.linearVelocityY > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * 0.5f);
        }

        if (context.performed && isWallSliding)
        {
            isWallSliding = false;
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);

            if (horizontalDirection != wallJumpDirection)
            {
                Flip();
            }

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
        }
    }

    public void ProcessGravity()
    {
        if (rb.linearVelocityY < 0)
        {
            rb.gravityScale = baseGravity * fallGravityMult;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, Mathf.Max(rb.linearVelocityY, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    public void ProcessWallSlide()
    {
        if (!IsGrounded() & WallCheck())
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(0, Mathf.Min(rb.linearVelocityY, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    public void ProcessWallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            rb.linearVelocity = new Vector2(0, 0);
            wallJumpDirection = -horizontalDirection;

            CancelInvoke(nameof(CancelWallJump));
        }
    }

    public void CancelWallJump()
    {
        isWallJumping = false;
    }

    public void Flip()
    {
        horizontalDirection*=-1;
        GetComponent<SpriteRenderer>().flipX = horizontalDirection==1 ? false : true;
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapBox(transform.position + new Vector3(0, GetComponent<CapsuleCollider2D>().size.y/-2, 0), groundCheckSize, 0, groundLayer);
    }

    public bool WallCheck()
    {
        return Physics2D.OverlapBox(transform.position + new Vector3(GetComponent<CapsuleCollider2D>().size.x/2 * horizontalDirection, 0, 0), wallCheckSize, 0, groundLayer);
    }
}