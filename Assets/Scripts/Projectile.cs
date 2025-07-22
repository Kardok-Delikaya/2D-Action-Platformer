using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    Rigidbody2D rb;

    [Header("Projectile")]
    public float physicalDamage = 5;
    public float magicDamage;
    public float stunDuration;
    public float pushForce;
    public Vector2 moveVelocity = new Vector2(5, 0);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    void Update()
    {
        rb.linearVelocity = moveVelocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerManager>() != null)
        {
            collision.GetComponent<PlayerCombatManager>().GetHit(physicalDamage, magicDamage, transform.position.x, stunDuration, pushForce);
        }

        gameObject.SetActive(false);
    }
}