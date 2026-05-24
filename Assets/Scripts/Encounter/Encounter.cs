<<<<<<< HEAD
using System.Threading.Tasks;
=======
using System;
using DefaultNamespace;
>>>>>>> 8bcbff50300ff83b41b436eb7ff9bf603eabaab8
using UnityEngine;
using UnityEngine.SceneManagement;

public class Encounter : MonoBehaviourID
{
    [SerializeField] private CombatEncounter combatEncounter;
    [SerializeField] private GameObject endPlayerPosition;

    private void Start()
    {
        if (EncounterProgressManager.Instance.IsEncounterDefeated(Guid.Parse(_id.Value)))
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EncounterProgressManager.Instance.ChangeCurrentEncounter(Guid.Parse(_id.Value));
            CombatManager.Instance.SetEncounter(combatEncounter);

<<<<<<< HEAD
            FadeToCombatScene();
=======
            CustomSceneManager.Instance.ChangeScene("TurnCombatScene");
>>>>>>> 8bcbff50300ff83b41b436eb7ff9bf603eabaab8
        }
    }

    async void FadeToCombatScene()
    {
        await FadeManager.Instance.FadeToBlack();
        
        await SceneManager.LoadSceneAsync("TurnCombatScene");

        await FadeManager.Instance.FadeToTransparent();
    }
}
