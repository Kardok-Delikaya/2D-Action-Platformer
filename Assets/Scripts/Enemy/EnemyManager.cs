using ActionPlatformer;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class EnemyManager : MonoBehaviour
{
    protected Vector3 startPos;
    protected PlayerManager player;
    protected Rigidbody2D rb;
    protected Animator anim;

    [Header("Enemy Stats")]
    public int maxHealth = 30;
    public int speed = 3;
    protected float speedUpTimer;
    int health;

    [Header("Enemy Damage")]
    public float physicalDamage = 3;
    public float magicDamage = 0;

    [Header("Enemy Defence")]
    public float physicalDefence;
    public float magicDefence;
    public float holyDefence;

    [Header("Enemy Status")]
    public bool isDead;
    public bool isAttacking;
    public bool isStunned;
    public bool isBeingPushed;
    public bool isInvulnerable;
    public bool canBeStunned = true;
    public bool canBePushed = true;
    public bool canParry;
    public bool canJump;

    [Header("UI")]
    public Slider healthSlider;

    [Header("Roll")]
    public bool canRoll;
    public bool hasRolledRollChance;
    public float rollChance;
    public float rollSpeed;
    public float rollDuration;

    [Header("Agro Settings")]
    public Vector2 agroArea = new Vector2(16, 4);
    public bool inCombat;
    public LayerMask playerLayer;
    protected float distanceFromPlayer;

    [Header("Attack Timer")]
    public float attackCoolDown = 5;
    protected float attackTimer;

    [Header("Facing Direction")]
    public bool isFacingRight = true;
    public int horizontalDirection = 1;

    [Header("Destination")]
    public float destinationX;
    public float maxWaitAroundTimer = 3f;
    protected float waitAroundTimer;

    [Header("Gravity")]
    public float baseGravity = 1f;
    public float maxFallSpeed = 32f;
    public float fallGravityMult = 2f;

    [Header("Ground Check")]
    public float characterLength = 1;
    public float characterSize = 1;
    public Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
    public Vector2 groundCheckFrontSize = new Vector2(0.03f, 0.03f);
    public LayerMask groundLayer;

    void Awake()
    {
        health = maxHealth;
        SetMaxHealthSliderValue();
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = FindAnyObjectByType<PlayerManager>();
    }

    private void Update()
    {
        anim.SetFloat("VelocityX", rb.linearVelocityX);
    }

    #region Combat
    public void CheckForEnemy()
    {
        if (inCombat)
            return;

        Collider2D playerCollider;
        playerCollider = Physics2D.OverlapBox(new Vector2(transform.position.x + horizontalDirection * agroArea.x / 2, transform.position.y + agroArea.y / 2), agroArea, 0, playerLayer);

        if (playerCollider != null)
        {
            inCombat = true;
        }
    }

    public virtual void GetHit(float physicalDamage, float magicDamage, float holyDamage, float hitDirection, float stunDuration = 0, float pushForce = 0)
    {
        if (isInvulnerable || isDead)
            return;

        //Turns to damage direction
        if (transform.position.x - hitDirection >= 0)
        {
            hitDirection = -1;

            if (isFacingRight) Flip();
        }
        else
        {
            hitDirection = 1;

            if (!isFacingRight) Flip();
        }

        //Damage gets caltulated
        float finalDamage = physicalDamage * (1 - physicalDefence / 100) + magicDamage * (1 - magicDefence / 100) + holyDamage * (1 - holyDefence / 100);

        health -= Mathf.RoundToInt(finalDamage);

        if (health > 0)
        {
            anim.Play("Hit_Reaction");

            if (canBePushed && pushForce != 0)
                StartCoroutine(PushBackCoroutine(Mathf.RoundToInt(-pushForce * hitDirection)));

            if (canBeStunned && stunDuration != 0)
                StartCoroutine(StunCoroutine(stunDuration));
        }
        else
        {
            health = 0;
            Die(hitDirection);
        }

        isAttacking = false;
        attackTimer = attackCoolDown;
        ChangeHealthSliderValue();
    }

    public void DamageEnemyWhenTouched()
    {
        attackTimer -= Time.deltaTime;

        if (Physics2D.OverlapBox(transform.position, transform.localScale, 0, playerLayer))
        {
            if (attackTimer <= 0)
            {
                anim.Play("Touch_Attack");
                player.playerCombat.GetHit(physicalDamage, magicDamage, transform.position.x, .3f, 20);
                rb.linearVelocityX = -speed * horizontalDirection;
                attackTimer = attackCoolDown;
                speedUpTimer = 0;
            }
        }
    }

    public virtual void GetParried()
    {
        StartCoroutine(StunCoroutine(2f));
    }

    public virtual void Die(float hitDirection)
    {
        isDead = true;
        inCombat = false;
        anim.Play("Die");
        Invoke("Revive",5f);
    }

    public virtual void Revive()
    {
        isDead = false;
        health = maxHealth;
        ChangeHealthSliderValue();
        anim.Play("Idle");
    }

    public IEnumerator PushBackCoroutine(int pushForce)
    {
        isBeingPushed = true;
        rb.linearVelocity = new Vector2(pushForce, 0);
        StartCoroutine(FindAnyObjectByType<CameraShakeEffect>().ShakeCameraCorutine(Mathf.Abs(pushForce)/5, .2f));

        yield return new WaitForSeconds(.3f);

        isBeingPushed = false;
        rb.linearVelocityX = 0;
    }

    public IEnumerator StunCoroutine(float stunDuration)
    {
        isStunned = true;

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
    }

    public IEnumerator Roll()
    {
        float currenntRollChance = Random.Range(0, 100);

        if (currenntRollChance < rollChance)
        {
            anim.Play("Roll");
            isBeingPushed = true;
            float dashDirection = isFacingRight ? 1f : -1f;

            isInvulnerable = true;
            rb.linearVelocity = new Vector2(dashDirection * rollSpeed, rb.linearVelocityY);

            yield return new WaitForSeconds(rollDuration);
            isInvulnerable = false;
            isBeingPushed=false;
            rb.linearVelocityX = 0;
        }

        yield return new WaitForSeconds(1f);

        hasRolledRollChance = false;
    }
    #endregion

    #region Movement
    public void MoveAroundThePlatform()
    {
        anim.Play("Walk");
        CheckForEnemy();

        if (Mathf.Abs(transform.position.x - destinationX) > .3f || !WillBeGrounded() || WillTouchWall())
        {
            if (transform.position.x - destinationX >= 0)
            {
                if (isFacingRight) Flip();
            }
            else
            {
                if (!isFacingRight) Flip();
            }

            rb.linearVelocityX = speed * horizontalDirection;
        }
        else
        {
            rb.linearVelocityX = 0;

            waitAroundTimer -= Time.deltaTime;

            if (waitAroundTimer < 0)
                SelectRandomDestination();
        }
    }

    public void SelectRandomDestination()
    {
        float distance = Random.Range(-9, 9);

        destinationX = startPos.x + distance;

        waitAroundTimer = maxWaitAroundTimer;
    }

    public void MoveToPlayer()
    {
        if (!WillBeGrounded() || WillTouchWall() || attackTimer > 0)
        {
            rb.linearVelocityX = 0;
            return;
        }

        anim.Play("Walk");
        rb.linearVelocityX = speed * horizontalDirection;
    }

    public void MoveOnPlatform()
    {
        anim.Play("Walk");

        if (speedUpTimer < 1)
        {
            speedUpTimer += Time.deltaTime;
            rb.linearVelocityX = Mathf.Lerp(0, speed * horizontalDirection, speedUpTimer);
        }
        else
        {
            rb.linearVelocityX = speed * horizontalDirection;
        }

        if (!WillBeGrounded() || WillTouchWall())
            Flip();
    }

    public void FaceToPlayer()
    {
        if (transform.position.x - player.transform.position.x >= 0)
        {
            if (isFacingRight) Flip();
        }
        else
        {
            if (!isFacingRight) Flip();
        }
    }

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        GetComponent<SpriteRenderer>().flipX = !isFacingRight;
        horizontalDirection = isFacingRight ? 1 : -1;
    }

    public void HandleGravity()
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
    #endregion

    #region Ground Checks
    public bool IsGrounded()
    {
        return Physics2D.OverlapBox(transform.position + new Vector3(0, -characterLength / 2, 0), groundCheckSize, 0, groundLayer);
    }

    public bool WillBeGrounded()
    {
        return Physics2D.OverlapBox(transform.position + new Vector3(horizontalDirection * characterSize / 2, -characterLength / 2, 0), groundCheckFrontSize, 0, groundLayer);
    }

    public bool WillTouchWall()
    {
        return Physics2D.OverlapBox(transform.position + new Vector3(horizontalDirection * characterSize / 2, 0, 0), groundCheckFrontSize, 0, groundLayer);
    }
    #endregion

    #region UI
    public void SetMaxHealthSliderValue()
    {
        healthSlider.maxValue = maxHealth;
        ChangeHealthSliderValue();
    }

    public void ChangeHealthSliderValue()
    {
        if (health == 0)
            healthSlider.gameObject.SetActive(false);
        else
            healthSlider.gameObject.SetActive(true);

        healthSlider.value = health;
    }
    #endregion
}