using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                GetComponent<BoxCollider2D>().enabled = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
            }
        }
}
