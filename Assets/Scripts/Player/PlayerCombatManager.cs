using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatManager : MonoBehaviour
{
    PlayerManager player;

    [Header("Player Stats")]
    [SerializeField] int maxHealth = 300;
    int currentHealth;
    [SerializeField] int maxMana = 100;
    int currentMana;
    [SerializeField] int maxUltimate = 100;
    int currentUltimate;
    [SerializeField] int maxPotion = 2;
    int currentPotion;

    [Header("Player Defence")]
    [SerializeField] float physicalDefence;
    [SerializeField] float magicDefence;
    [SerializeField] float blockDefence;

    [Header("Player Damage")]
    [SerializeField] float physicalDamageMultiplier=1;
    [SerializeField] float magicDamageMultiplier=1;
    [SerializeField] float holyDamageMultiplier = 1;

    [Header("Attack Zones")]
    [SerializeField] Transform damageZone;
    [SerializeField] Transform parryZone;

    [Header("Enemy Layer")]
    [SerializeField] LayerMask enemyLayer;

    [Header("Attack List")]
    [SerializeField] AttackActions lightAttack01;
    [SerializeField] AttackActions lightAttack02;
    [SerializeField] AttackActions lightAttack03;
    [SerializeField] AttackActions heavyAttack01;
    [SerializeField] AttackActions heavyAttack02;
    [SerializeField] AttackActions magicAttack01;
    AttackActions currentAttackAction;

    [Header("Last Attack")]
    [SerializeField] AttackActions lastAttack;

    [Header("Player Status")]
    public bool isBlocking;
    public bool isInvulnerable;
    public bool isStunned;
    public bool isAttacking;

    [Header("Timers")]
    [SerializeField] float maxParryTimer = .1f;
    [SerializeField] float maxComboTimer = 1f;
    float parryTimer;
    float comboTimer;

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentUltimate = maxUltimate;
        currentPotion = maxPotion;
        player.uiManager.HandleUIStart(maxHealth, maxMana, maxUltimate, maxPotion, currentHealth, currentMana, currentUltimate, currentPotion);
    }

    private void Update()
    {
        parryTimer -= Time.deltaTime;
        comboTimer -= Time.deltaTime;
    }

    public void DealDamage()
    {
        lastAttack = currentAttackAction;
        StartComboTimer();

        Collider2D[] enemies = Physics2D.OverlapBoxAll(damageZone.position, damageZone.localScale, 0, enemyLayer);

        if (enemies.Length > 0)
        {
            for (int i = 0; i < enemies.Length; i++)
                enemies[i].GetComponent<EnemyManager>().GetHit(currentAttackAction.physicalDamage * physicalDamageMultiplier,
                    currentAttackAction.magicDamage * magicDamageMultiplier, currentAttackAction.holyDamage * holyDamageMultiplier, 
                    transform.position.x, currentAttackAction.stunDuration, currentAttackAction.pushForce);
        }
    }

    public void GetHit(float physicalDamage, float magicDamage, float hitDirection, float stunDuration=0, float pushForce = 0)
    {
        if (isInvulnerable)
            return;

        if (isBlocking)
            player.playerAnimation.PlayAnimation("Block_Hit_Reaction");
        else
            player.playerAnimation.PlayAnimation("Hit_Reaction");

        if (transform.position.x - hitDirection > 0)
        {
            hitDirection = -1;

            if (player.playerMovement.horizontalDirection==1) player.playerMovement.Flip();
        }
        else
        {
            hitDirection = 1;

            if (player.playerMovement.horizontalDirection==-1) player.playerMovement.Flip();
        }

        if (parryTimer > 0 && hitDirection == player.playerMovement.horizontalDirection)
        {
            Collider2D[] enemies = Physics2D.OverlapBoxAll(parryZone.transform.position, parryZone.transform.localScale, 0, enemyLayer);

            if (enemies.Length == 0)
                return;

            for (int i = 0; i < enemies.Length; i++)
                enemies[i].GetComponent<EnemyManager>().GetParried();

            return;
        }

        float finalPhysicalDamage = player.skillTree.armorLevel > 2 ? physicalDamage * (1 - (physicalDefence + 10) / 100) : physicalDamage * (1 - physicalDefence / 100);
        float finalMagicDamage = player.skillTree.armorLevel > 4 ? magicDamage * (1 - (magicDefence + 10) / 100) : magicDamage * (1 - magicDefence / 100);

        float finalDamage = finalPhysicalDamage + finalMagicDamage;

        if (isBlocking && hitDirection == player.playerMovement.horizontalDirection) finalDamage *= player.skillTree.lessDamageWhileBlocking == true ? (blockDefence + 10) / 100 : blockDefence / 100;

        currentHealth -= Mathf.RoundToInt(finalDamage);

        if (currentHealth > 0)
        {
            if (pushForce > 0)
                StartCoroutine(player.playerMovement.PushBackCoroutine(Mathf.RoundToInt(-pushForce * hitDirection)));

            if (stunDuration > 0)
                StartCoroutine(StunCoroutine(stunDuration));
        }
        else
        {
            currentHealth=0;
            Die();
        }

        player.uiManager.UpdateHealthSlider(currentHealth);

        FindAnyObjectByType<HitStop>().HitStopEffect(.2f);
        StartCoroutine(FindAnyObjectByType<CameraShakeEffect>().ShakeCameraCorutine(2, .2f));
    }

    void Die()
    {
        player.isDead = true;
        player.isInteracting = true;
        player.playerAnimation.PlayAnimation("Die");
    }

    #region Handle Actions
    public void HandleBlock(InputAction.CallbackContext context)
    {
        if (context.started && !player.isInteracting)
        {
            isAttacking = true;
            player.playerAnimation.PlayAnimation("Block_Start");
        }

        if (context.canceled)
        {
            isAttacking = false;
            player.playerAnimation.PlayAnimation("Block_End");
        }
    }

    public void HandleLightAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            damageZone.localPosition = new Vector3(player.playerMovement.horizontalDirection * Mathf.Abs(damageZone.localPosition.x), damageZone.localPosition.y);

            if (!player.isInteracting)
            {
                isAttacking = true;
                currentAttackAction = lightAttack01;
                player.playerAnimation.PlayAnimation(currentAttackAction.attackAnimation);

            }
            else if (comboTimer>0 && lastAttack==lightAttack01)
            {
                currentAttackAction = lightAttack02;
                player.playerAnimation.PlayAnimation(currentAttackAction.attackAnimation);
            }
            else if (comboTimer > 0 && lastAttack==lightAttack02)
            {
                currentAttackAction = lightAttack03;
                player.playerAnimation.PlayAnimation(currentAttackAction.attackAnimation);
            }
        }
    }

    public void HandleHeavyAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            damageZone.localPosition = new Vector3(player.playerMovement.horizontalDirection * Mathf.Abs(damageZone.localPosition.x), damageZone.localPosition.y);

            if (!player.isInteracting)
            {
                isAttacking = true;
                currentAttackAction = heavyAttack01;
                player.playerAnimation.PlayAnimation(currentAttackAction.attackAnimation);
            }
            else if (comboTimer > 0 && lastAttack==heavyAttack01)
            {
                currentAttackAction = heavyAttack02;
                player.playerAnimation.PlayAnimation(currentAttackAction.attackAnimation);
            }
        }
    }

    public void HandleMagicAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            damageZone.localPosition = new Vector3(player.playerMovement.horizontalDirection * Mathf.Abs(damageZone.localPosition.x), damageZone.localPosition.y);

            if (player.isInteracting)
                return;

            if (currentMana > 15)
            {
                isAttacking = true;
                currentAttackAction = magicAttack01;
                player.playerAnimation.PlayAnimation(currentAttackAction.attackAnimation);
                currentMana -= 15;
                player.uiManager.UpdateManaSlider(currentMana);
            }
        }
    }
    #endregion

    public IEnumerator StunCoroutine(float stunDuration)
    {
        isStunned = true;

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
    }

    #region Setting Values For Animation Event Keys
    public void SetInvulnarableTrue()
    {
        isInvulnerable = true;
    }

    public void SetInvulnarableFalse()
    {
        isInvulnerable = false;
    }

    public void SetIsBlockingTrue()
    {
        isBlocking = true;

        if (player.skillTree.canDoParry)
            parryTimer = player.skillTree.moreParryLength == true ? maxParryTimer * 2 : maxParryTimer;
    }

    public void SetIsBlockingFalse()
    {
        isBlocking = false;
    }

    public void StartComboTimer()
    {
        comboTimer = maxComboTimer;
    }
    #endregion
}