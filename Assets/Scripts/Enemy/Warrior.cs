using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class Warrior : EnemyManager
{
    [Header("Attack Zone")]
    [SerializeField] Transform attackZone;

    private void Start()
    {
        SelectRandomDestination();
    }

    void Update()
    {
        if (isDead||isBeingPushed) return;

        if (inCombat)
        {
            distanceFromPlayer = Vector2.Distance(player.transform.position, transform.position);

            MoveToPlayer();

            AttackToPlayer();

            if(attackTimer > 0) 
                return;

            FaceToPlayer();

            if (player.playerCombat.isAttacking && distanceFromPlayer < 5 && !hasRolledRollChance)
            {
                hasRolledRollChance = true;
                StartCoroutine(Roll());
            }
        }
        else
        {
            MoveAroundThePlatform();
        }
    }

    void AttackToPlayer()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            return;
        }

        if (distanceFromPlayer < 1.5f)
        {
            attackZone.localPosition = new Vector3(horizontalDirection * Mathf.Abs(attackZone.localPosition.x), attackZone.localPosition.y);
            isAttacking = true;
            anim.Play("Light_Attack_01");
            attackTimer = attackCoolDown;
        }
    }

    public void DealDamage()
    {
        Collider2D attackedPlayer = Physics2D.OverlapBox(attackZone.position, attackZone.localScale, 0, playerLayer);

        if (attackedPlayer != null && isAttacking)
            player.playerCombat.GetHit(physicalDamage, magicDamage, transform.position.x, 0, 10);

        isAttacking=false;
    }

    public override void GetParried()
    {
        base.GetParried();

        anim.Play("Hit_Reaction");
    }

    public void StartComboTimer()
    {
        //Check if can do Combo
        //Roll for Combo chance
        //Make Combo Attack
    }
}