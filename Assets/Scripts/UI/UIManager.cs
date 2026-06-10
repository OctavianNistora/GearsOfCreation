using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VolumeSettings volumeSettings;
    
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OpenVolumeSettings()
    {
        volumeSettings.gameObject.SetActive(true);
    }
}
