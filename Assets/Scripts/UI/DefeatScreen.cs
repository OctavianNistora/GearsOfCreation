using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatScreen : MonoBehaviour
{
    public static DefeatScreen Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async void FadeToPlatformerScene()
    {
        gameObject.SetActive(false);

        await FadeManager.Instance.FadeToBlack();

        await SceneManager.LoadSceneAsync("PlatformerScene");

        await FadeManager.Instance.FadeToTransparent();
    }

    public async void FadeToMainMenu()
    {
        gameObject.SetActive(false);

        await FadeManager.Instance.FadeToBlack();

        await SceneManager.LoadSceneAsync("Main_Menu");

        await FadeManager.Instance.FadeToTransparent();
    }
}
