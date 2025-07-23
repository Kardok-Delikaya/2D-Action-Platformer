using Unity.VisualScripting;
using UnityEngine;

public class Archer : EnemyManager
{
    [Header("Arrow")]
    [SerializeField] Projectile arrow;
    [SerializeField] float stunDuration;
    [SerializeField] float pushForce;
    [SerializeField] int horizantalArrowSpeed;
    [SerializeField] int verticalArrowSpeed;

    void Update()
    {
        if (isDead || isBeingPushed) return;

        if (!inCombat)
        {
            CheckForEnemy();
        }
        else
        {
            distanceFromPlayer = Vector2.Distance(player.transform.position, transform.position);

            if (player.playerCombat.isAttacking && distanceFromPlayer < 5 && !hasRolledRollChance)
            {
                hasRolledRollChance = true;
                StartCoroutine(Roll());
            }

            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }
            else if (Mathf.Abs(player.transform.position.y - transform.position.y) < 0.5f)
            {
                isAttacking = true;
                anim.Play("Shoot_Arrow");
                attackTimer = attackCoolDown;
            }

            if (isAttacking)
                return;

            FaceToPlayer();
        }
    }

    void ReleaseArrow()
    {
        if (!isStunned)
        {
            arrow.gameObject.SetActive(true);
            arrow.transform.position = new Vector2(transform.position.x + horizontalDirection * characterSize / 2, transform.position.y);
            arrow.physicalDamage = physicalDamage;
            arrow.magicDamage = magicDamage;
            arrow.stunDuration = stunDuration;
            arrow.pushForce = pushForce;
            arrow.moveVelocity = new Vector2(horizantalArrowSpeed * horizontalDirection, verticalArrowSpeed);
            isAttacking = false;
        }
    }
}