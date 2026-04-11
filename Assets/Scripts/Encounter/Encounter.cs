using UnityEngine;
using UnityEngine.SceneManagement;

public class Encounter : MonoBehaviour
{
    [SerializeField] GameObject enemiesParent;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {          
            EncounterManager.Instance.enemiesToSpawn.Clear();
            foreach (Transform childTransform in enemiesParent.transform)
            {
                EncounterEnemy encounterEnemy = childTransform.GetComponent<EncounterEnemy>();
                EncounterManager.Instance.enemiesToSpawn.Add(encounterEnemy.enemyData);
            }

            SceneManager.LoadScene("TurnCombatScene");
        }
    }
}
