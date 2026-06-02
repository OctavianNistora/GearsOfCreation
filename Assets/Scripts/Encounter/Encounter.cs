using System.Threading.Tasks;
using System;
using DefaultNamespace;

using UnityEngine;
using UnityEngine.SceneManagement;

public class Encounter : MonoBehaviourID
{
    [SerializeField] private CombatEncounter combatEncounter;
    [SerializeField] private GameObject playerPositionAfterCombat;
    [SerializeField] private EncounterType encounterType;

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

            //CustomSceneManager.Instance.ChangeScene("TurnCombatScene");
            CombatManager.Instance.currentEncounterType = encounterType;
            CombatManager.Instance.playerPositionAfterCombat = playerPositionAfterCombat.transform.position;
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

public enum EncounterType
{
    Tutorial,
    GlassyWaters
}
