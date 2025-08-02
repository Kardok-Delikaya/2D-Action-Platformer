using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMovementManager : MonoBehaviour
{
    Rigidbody2D rb;
    PlayerManager player;
    SpriteRenderer sprite;
    CapsuleCollider2D col2D;
    bool beingPushed;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 10f;
    public int horizontalDirection = 1;
    float horizontalMovement;
    float verticalMovement;

    [Header("Rolling")]
    [SerializeField] float rollSpeed = 20f;
    [SerializeField] float rollDuration = 0.25f;
    [SerializeField] float rollCoolDown = 1f;
    [SerializeField] bool isRolling;
    bool canRoll = true;

    [Header("Jump")]
    [SerializeField] float jumpPower = 18f;
    public bool canWallJump;

    [Header("Ground Check")]
    [SerializeField] Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
    [SerializeField] Vector2 wallCheckSize = new Vector2(0.03f, 0.49f);
    [SerializeField] LayerMask groundLayer;

    [Header("Gravity")]
    [SerializeField] float baseGravity = 3f;
    [SerializeField] float maxFallSpeed = 18f;
    [SerializeField] float fallGravityMult = 3f;

    [Header("Wall Movement")]
    [SerializeField] float wallSlideSpeed = 2f;
    [SerializeField] Vector2 wallJumpPower = new Vector2(10f, 20f);
    bool isWallSliding;
    bool isWallJumping;
    float wallJumpDirection;

    void Awake()
    {
        col2D = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<PlayerManager>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        ProcessGravity();

        if (isRolling || player.playerCombat.isStunned || player.isDead || beingPushed)
            return;

        if (canWallJump)
        {
            ProcessWallSlide();
            ProcessWallJump();
        }

        if (!isWallJumping)
        {
            if (player.playerCombat.isAttacking)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
            }
            else
            {
                rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocityY);

                if (horizontalDirection == 1 && horizontalMovement < 0 || horizontalDirection == -1 && horizontalMovement > 0)
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
        beingPushed = false;
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

            Invoke(nameof(CancelWallJump), 0.5f);
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
        horizontalDirection *= -1;
        sprite.flipX = horizontalDirection == 1 ? false : true;
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapBox(transform.position + new Vector3(0, col2D.size.y / -2, 0), groundCheckSize, 0, groundLayer);
    }

    public bool WallCheck()
    {
        return Physics2D.OverlapBox(transform.position + new Vector3(col2D.size.x / 2 * horizontalDirection, 0, 0), wallCheckSize, 0, groundLayer);
    }
}