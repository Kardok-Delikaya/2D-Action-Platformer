using UnityEngine;
using DG.Tweening;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform pointToMove;
    [SerializeField] private float timeToMove = 5f;

    private void Start()
    {
        transform.DOMove(pointToMove.position, timeToMove).SetLoops(-1,LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.transform.SetParent(transform);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.transform.parent == null)
            return;

        collision.gameObject.transform.SetParent(null);
    }
}