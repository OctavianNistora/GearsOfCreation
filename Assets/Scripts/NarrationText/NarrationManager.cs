using TMPro;
using UnityEngine;

public class NarrationManager : MonoBehaviour
{
    public TextMeshProUGUI textUI;

    public void ShowText(string message)
    {
        textUI.text = message;
        textUI.gameObject.SetActive(true);
    }

    public void HideText()
    {
        textUI.gameObject.SetActive(false);
    }
}
