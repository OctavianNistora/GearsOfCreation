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
        SceneManager.LoadSceneAsync("PlatformerScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
