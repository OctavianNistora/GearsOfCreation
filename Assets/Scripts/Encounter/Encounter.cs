using UnityEngine;
using UnityEngine.SceneManagement;

public class Encounter : MonoBehaviour
{
    [SerializeField] private CombatEncounter combatEncounter;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CombatManager.Instance.SetEncounter(combatEncounter);

            SceneManager.LoadScene("TurnCombatScene");
        }
    }
}
