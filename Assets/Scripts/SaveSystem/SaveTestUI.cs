using UnityEngine;
using UnityEngine.UI;

public class SaveTestUI : MonoBehaviour
{
    // Reference to the player originator script in your scene
    [SerializeField] private PlayerOriginator playerOriginator;

    /// <summary>
    /// Hook this up to your UI "Save Button" OnClick() event
    /// </summary>
    public void OnSaveButtonClicked()
    {
        if (playerOriginator != null)
        {
            // We pass a dummy checkpoint name for testing
            //playerOriginator.SaveGameState("Test_ManualSave_Checkpoint");
            Debug.Log("[TestUI] Save Button Triggered!");
        }
        else
        {
            Debug.LogError("[TestUI] Cannot save: PlayerOriginator reference is missing!");
        }
    }

    /// <summary>
    /// Hook this up to your UI "Load Button" OnClick() event
    /// </summary>
    public void OnLoadButtonClicked()
    {
        if (playerOriginator != null)
        {
            if (SaveSystem.HasSave())
            {
                //playerOriginator.LoadGameState();
                Debug.Log("[TestUI] Load Button Triggered!");
            }
            else
            {
                Debug.LogWarning("[TestUI] Cannot load: No save file exists yet.");
            }
        }
        else
        {
            Debug.LogError("[TestUI] Cannot load: PlayerOriginator reference is missing!");
        }
    }
}