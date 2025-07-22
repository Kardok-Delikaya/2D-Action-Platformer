using UnityEngine;

public class FlyingEye : EnemyManager
{
    void Update()
    {
        rb.linearVelocityY = 0;
    }
}
