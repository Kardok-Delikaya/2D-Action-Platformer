using UnityEngine;

public class Crawler : EnemyManager
{
    private void Update()
    {
        if (isDead || isBeingPushed || isStunned) return;

        if (IsGrounded())
        {
            DamageEnemyWhenTouched();

            if (attackTimer > 0 || isStunned)
                return;

            MoveOnPlatform();
        }
        else
        {
            HandleGravity();
        }
    }

    public override void GetHit(float physicalDamage, float magicDamage, float holyDamage, float hitDirection, float stunDuration = 0, float pushForce = 0)
    {
        base.GetHit(physicalDamage, magicDamage, holyDamage, hitDirection, stunDuration, pushForce);

        speedUpTimer = 0;
    }
}