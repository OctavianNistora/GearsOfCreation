using UnityEngine;

public class EncounterTypeAdapter : MonoBehaviour
{
    [SerializeField] private GameObject tutorialBackground;
    [SerializeField] private GameObject glassyWatersBackground;
    
    [Header ("Party Members")]
    [SerializeField] private PlayerEntity WaterFairy;

    private void Start()
    {
        switch (CombatManager.Instance.currentEncounterType)
        {
            case EncounterType.Tutorial:
                tutorialBackground.SetActive(true);
                glassyWatersBackground.SetActive(false);
                break;
            case EncounterType.GlassyWaters:
                if (!PartyManager.Instance.Members.Contains(WaterFairy))
                    PartyManager.Instance.Members.Add(WaterFairy);
                    
                tutorialBackground.SetActive(false);
                glassyWatersBackground.SetActive(true);
                break;
            default:
                Debug.LogError("Unknown encounter type: " + CombatManager.Instance.currentEncounterType);
                break;
        }
    }
}
