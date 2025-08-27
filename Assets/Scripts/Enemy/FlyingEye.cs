using UnityEngine;

public class FlyingEye : EnemyManager
{
    private void Update()
    {
        rb.linearVelocityY = 0;
    }
}
