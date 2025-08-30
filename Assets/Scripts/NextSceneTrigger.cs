using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneTrigger : MonoBehaviour
{
    public static NextSceneTrigger Instance;
    
    private bool allIsDead=false;
    public List<EnemyManager> enemies;
    
    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        CheckIfAllEnemiesAreDead();
    }

    public void CheckIfAllEnemiesAreDead()
    {

        if (enemies.Count > 0)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].isDead)
                {
                    allIsDead = false;
                    return;
                }

                allIsDead = true;
            }
        }
        else
        {
            allIsDead = true;
        }

        if(allIsDead) GetComponent<BoxCollider2D>().isTrigger = true;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerCombatManager>().HandlePlayerStatSave();
            GetComponent<BoxCollider2D>().enabled = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}