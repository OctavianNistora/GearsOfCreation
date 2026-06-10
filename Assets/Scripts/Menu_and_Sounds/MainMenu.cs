using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.PlayMusic(AudioManager.Instance.mainMenu);
    }

    public void PlayGame()
    {
        FadeToPlatformerScene();
    }

    public void OpenSettings()
    {
        VolumeSettings.Instance.previousUI = gameObject;
        VolumeSettings.Instance.children.SetActive(true);
        gameObject.SetActive(false);
    }

    public async void FadeToPlatformerScene()
    {
        await FadeManager.Instance.FadeToBlack();

        await SceneManager.LoadSceneAsync("PlatformerScene");

        await FadeManager.Instance.FadeToTransparent();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
