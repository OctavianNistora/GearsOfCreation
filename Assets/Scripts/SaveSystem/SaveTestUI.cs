using UnityEngine;
using UnityEngine.UI;

public class SaveTestUI : MonoBehaviour
{
    // Reference to the player originator script in your scene
    [SerializeField] private PlayerOriginator playerOriginator;

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
                playerOriginator.LoadGameState();
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