using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
    [SerializeField] private int bounceForce = 15;

    [Header("Damageable Platform")]
    [SerializeField] private bool canDoDamage = false;
    [SerializeField] private int damage = 25;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerManager>() != null)
        {
            if (other.gameObject.GetComponent<PlayerManager>().isDead) return;
            
            HandlePlayerBounce(other.gameObject);
        }
        else if (other.gameObject.GetComponent<EnemyManager>() != null)
        {
            if (other.gameObject.GetComponent<EnemyManager>().isDead) return;
            
            HandleEnemyBounce(other.gameObject);
        }
    }

    private void HandlePlayerBounce(GameObject player)
    {
        player.GetComponent<Rigidbody2D>().linearVelocityY = 0;
        player.GetComponent<Rigidbody2D>().AddForceY(bounceForce, ForceMode2D.Impulse);

        if (!canDoDamage) return;
        
        player.GetComponent<PlayerCombatManager>().GetHit(damage, 0, player.transform.position.x);
        player.GetComponent<PlayerCombatManager>().isInvulnerable=true;
    }

    private void HandleEnemyBounce(GameObject enemy)
    {
        if (canDoDamage)
        {
            enemy.GetComponent<EnemyManager>().GetHit(10000,0, 0,transform.position.x);
        }
        else
        {
            enemy.GetComponent<Rigidbody2D>().linearVelocityY = 0;
            enemy.GetComponent<Rigidbody2D>().AddForceY(bounceForce, ForceMode2D.Impulse);
        }
    }
}
