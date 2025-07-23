using UnityEngine;

public class Crawler : EnemyManager
{
    private void Update()
    {
        if (isDead || isBeingPushed) return;

        if (IsGrounded())
        {
            if (attackTimer > 0 || isStunned)
                return;

            MoveOnPlatform();
            DamageEnemyWhenTouched();
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