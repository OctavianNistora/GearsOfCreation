using UnityEngine;

public class ZoneBorder : MonoBehaviour
{
    [SerializeField]
    GameObject objectsUp;
    [SerializeField]
    GameObject objectsDown;
    [SerializeField]
    PlayerMovement player;

    void Start()
    {
        if(player.transform.position.y < gameObject.transform.position.y)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.glassyWaters);
            objectsUp.SetActive(false);
            objectsDown.SetActive(true);
        }
        else
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.tutorial);
            objectsUp.SetActive(true);
            objectsDown.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // if player descends through border
        if(collision.transform.position.y < gameObject.transform.position.y)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.glassyWaters);
            objectsUp.SetActive(false);
            objectsDown.SetActive(true);
        }
        else
        {
            // if player climbs through border
            AudioManager.Instance.PlayMusic(AudioManager.Instance.tutorial);
            objectsUp.SetActive(true);
            objectsDown.SetActive(false);
        }
    }
}
