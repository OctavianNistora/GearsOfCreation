using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    public GameObject children;
    
    public static VictoryScreen Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        children.SetActive(false);
    }

    public void QuitToMainMenu()
    {
        FadeToMainMenu();
    }

    public async void FadeToMainMenu()
    {
        AudioManager.Instance.StopSFX();

        children.SetActive(false);

        await FadeManager.Instance.FadeToBlack();

        await SceneManager.LoadSceneAsync("Main_Menu");

        await FadeManager.Instance.FadeToTransparent();
    }
}
