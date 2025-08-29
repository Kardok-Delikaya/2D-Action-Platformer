using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    private BoxCollider2D col;
    private SpriteRenderer sprite;
    [SerializeField] private float disappearTime=5;
    [SerializeField] private float reappearTime=5;
    [SerializeField] private float disappearStartTime=0;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        
        if (disappearTime<=0)
            return;

        InvokeRepeating(nameof(Disappear), disappearStartTime, disappearTime+reappearTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (disappearTime>0) return;
        
        Invoke(nameof(Disappear), disappearTime);
    }

    private void Disappear()
    {
        col.enabled = false;
        sprite.enabled = false;
        
        Invoke(nameof(Reappear), reappearTime);
    }

    private void Reappear()
    {
        col.enabled = true;
        sprite.enabled = true;
    }
}