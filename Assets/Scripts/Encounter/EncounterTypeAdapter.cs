using UnityEngine;

public class EncounterTypeAdapter : MonoBehaviour
{
    [SerializeField] private GameObject tutorialBackground;
    [SerializeField] private GameObject glassyWatersBackground;
    
    [Header ("Party Members")]
    [SerializeField] private PlayerEntity Grandson;
    [SerializeField] private PlayerEntity WaterFairy;

    private void Start()
    {
        AudioManager.Instance.PlayMusic(AudioManager.Instance.combat);

        print(PartyManager.Instance.Members);
        switch (CombatManager.Instance.currentEncounterType)
        {
            case EncounterType.Tutorial:
                if (!PartyManager.Instance.Members.Contains(Grandson))
                    PartyManager.Instance.Members.Add(Grandson);

                tutorialBackground.SetActive(true);
                glassyWatersBackground.SetActive(false);
                break;
            case EncounterType.GlassyWaters:
                if (!PartyManager.Instance.Members.Contains(Grandson))
                    PartyManager.Instance.Members.Add(Grandson);
                    
                if (!PartyManager.Instance.Members.Contains(WaterFairy))
                    PartyManager.Instance.Members.Add(WaterFairy);
                    
                tutorialBackground.SetActive(false);
                glassyWatersBackground.SetActive(true);
                break;
            default:
                Debug.LogError("Unknown encounter type: " + CombatManager.Instance.currentEncounterType);
                break;
        }
        PartyManager.Instance.RestoreAllPartyMembersStats();
    }
}
