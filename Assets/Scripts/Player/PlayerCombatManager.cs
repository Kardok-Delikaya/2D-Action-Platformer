using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatManager : MonoBehaviour
{
    private PlayerManager player;

    [Header("Player Stats")]
    [SerializeField]
    private int maxHealth = 300;

    private int currentHealth;
    [SerializeField] private int maxMana = 100;
    private int currentMana;
    [SerializeField] private int maxUltimate = 100;
    private int currentUltimate;
    [SerializeField] private int maxPotion = 2;
    private int currentPotion;

    [Header("Player Defence")]
    [SerializeField]
    private float physicalDefence;
    [SerializeField] private float magicDefence;
    [SerializeField] private float blockDefence;

    [Header("Player Damage")]
    [SerializeField]
    private float physicalDamageMultiplier = 1;
    [SerializeField] private float magicDamageMultiplier = 1;
    [SerializeField] private float holyDamageMultiplier = 1;

    [Header("Attack Zones")]
    [SerializeField]
    private Transform damageZone;
    [SerializeField] private Transform parryZone;

    [Header("Enemy Layer")]
    [SerializeField]
    private LayerMask enemyLayer;

    [Header("Attack List")]
    [SerializeField]
    private AttackActions lightAttack01;
    [SerializeField] private AttackActions lightAttack02;
    [SerializeField] private AttackActions lightAttack03;
    [SerializeField] private AttackActions heavyAttack01;
    [SerializeField] private AttackActions heavyAttack02;
    [SerializeField] private AttackActions magicAttack01;
    private AttackActions currentAttackAction;

    [Header("Last Attack")]
    [SerializeField]
    private AttackActions lastAttack;

    [Header("Player Status")]
    public bool isBlocking;
    public bool isInvulnerable;
    public bool isStunned;
    public bool isAttacking;

    [Header("Timers")]
    [SerializeField]
    private float maxParryTimer = .1f;
    [SerializeField] private float maxComboTimer = 1f;
    private float parryTimer;
    private float comboTimer;

    private void Start()
    {
        player = GetComponent<PlayerManager>();
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentUltimate = maxUltimate;
        currentPotion = maxPotion;
        UIManager.Instance.HandleUIStart(maxHealth, maxMana, maxUltimate, maxPotion, currentHealth, currentMana, currentUltimate, currentPotion);
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

    public void GetHit(float physicalDamage, float magicDamage, float hitDirection, float stunDuration = 0, float pushForce = 0)
    {
        if (isInvulnerable || player.isDead)
            return;

        if (isBlocking)
            player.playerAnimation.PlayAnimation("Block_Hit_Reaction");
        else
            player.playerAnimation.PlayAnimation("Hit_Reaction");

        if (transform.position.x - hitDirection > 0)
        {
            hitDirection = -1;

            if (player.playerMovement.horizontalDirection == 1) player.playerMovement.Flip();
        }
        else if  (transform.position.x - hitDirection < 0)
        {
            hitDirection = 1;

            if (player.playerMovement.horizontalDirection == -1) player.playerMovement.Flip();
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

        if (isBlocking && hitDirection == player.playerMovement.horizontalDirection) finalDamage *= 1 - blockDefence / 100;

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
            currentHealth = 0;
            Die();
        }

        UIManager.Instance.UpdateHealthSlider(currentHealth);

        HitStop.Instance.HitStopEffect(.2f);
        StartCoroutine(CameraShakeEffect.Instance.ShakeCameraCorutine(2, .2f));
    }

    private void Die()
    {
        player.isDead = true;
        player.isInteracting = true;
        player.playerAnimation.PlayAnimation("Death");
    }

    #region Handle Actions
    public void HandleBlock(InputAction.CallbackContext context)
    {
        if (player.isDead) return;
        
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
        if (player.isDead) return;
        
        if (context.started)
        {
            damageZone.localPosition = new Vector3(player.playerMovement.horizontalDirection * Mathf.Abs(damageZone.localPosition.x), damageZone.localPosition.y);

            if (!player.isInteracting)
            {
                isAttacking = true;
                currentAttackAction = lightAttack01;
                player.playerAnimation.PlayAnimation(currentAttackAction.attackAnimation);

            }
            else if (comboTimer > 0 && lastAttack == lightAttack01)
            {
                currentAttackAction = lightAttack02;
                player.playerAnimation.PlayAnimation(currentAttackAction.attackAnimation);
            }
            else if (comboTimer > 0 && lastAttack == lightAttack02)
            {
                currentAttackAction = lightAttack03;
                player.playerAnimation.PlayAnimation(currentAttackAction.attackAnimation);
            }
        }
    }

    public void HandleHeavyAttack(InputAction.CallbackContext context)
    {
        if (player.isDead) return;
        
        if (context.started)
        {
            damageZone.localPosition = new Vector3(player.playerMovement.horizontalDirection * Mathf.Abs(damageZone.localPosition.x), damageZone.localPosition.y);

            if (!player.isInteracting)
            {
                isAttacking = true;
                currentAttackAction = heavyAttack01;
                player.playerAnimation.PlayAnimation(currentAttackAction.attackAnimation);
            }
            else if (comboTimer > 0 && lastAttack == heavyAttack01)
            {
                currentAttackAction = heavyAttack02;
                player.playerAnimation.PlayAnimation(currentAttackAction.attackAnimation);
            }
        }
    }

    public void HandleMagicAttack(InputAction.CallbackContext context)
    {
        if (player.isDead) return;
        
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
                UIManager.Instance.UpdateManaSlider(currentMana);
            }
        }
    }
    #endregion

    private IEnumerator StunCoroutine(float stunDuration)
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
            parryTimer = maxParryTimer;
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