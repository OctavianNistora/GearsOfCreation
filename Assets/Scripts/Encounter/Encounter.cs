using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Encounter : MonoBehaviour
{
    [SerializeField] private CombatEncounter combatEncounter;
    [SerializeField] private GameObject endPlayerPosition;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CombatManager.Instance.SetEncounter(combatEncounter);

            FadeToCombatScene();
        }
    }

    async void FadeToCombatScene()
    {
        await FadeManager.Instance.FadeToBlack();
        
        await SceneManager.LoadSceneAsync("TurnCombatScene");

        await FadeManager.Instance.FadeToTransparent();
    }
}
