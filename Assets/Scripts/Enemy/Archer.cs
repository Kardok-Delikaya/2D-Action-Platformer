using Unity.VisualScripting;
using UnityEngine;

public class Archer : EnemyManager
{
    [Header("Arrow")]
    [SerializeField]
    private GameObject arrow;
    [SerializeField] private Transform arrowSpawnPos;
    [SerializeField] private float stunDuration;
    [SerializeField] private float pushForce;
    [SerializeField] private int horizantalArrowSpeed;
    [SerializeField] private int verticalArrowSpeed;

    private void Update()
    {
        if (isDead || isBeingPushed || isStunned) return;

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

    private void ReleaseArrow()
    {
        if (!isStunned)
        {
            Projectile throwedArrow = Instantiate(arrow, arrowSpawnPos).GetComponent<Projectile>();
            throwedArrow.physicalDamage = physicalDamage;
            throwedArrow.magicDamage = magicDamage;
            throwedArrow.stunDuration = stunDuration;
            throwedArrow.pushForce = pushForce;
            throwedArrow.moveVelocity = new Vector2(horizantalArrowSpeed * horizontalDirection, verticalArrowSpeed);
            isAttacking = false;
        }
    }
}