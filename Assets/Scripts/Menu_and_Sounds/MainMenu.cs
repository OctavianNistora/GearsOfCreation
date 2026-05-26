using UnityEngine;
using UnityEngine.SceneManagement;
public class MaimMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("PlatformerScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }


}
