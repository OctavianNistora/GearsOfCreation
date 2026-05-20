using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Encounter : MonoBehaviourID
{
    [SerializeField] private CombatEncounter combatEncounter;

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

            CustomSceneManager.Instance.ChangeScene("TurnCombatScene");
        }
    }
}
