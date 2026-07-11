using TMPro;
using UnityEngine;

public class NarrationManager : MonoBehaviour
{
    public TextMeshProUGUI textUI;

    public static NarrationManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
