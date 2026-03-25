using UnityEngine;

public class ZoneBorder : MonoBehaviour
{
    [SerializeField]
    GameObject objectsUp;
    [SerializeField]
    GameObject objectsDown;
    void OnTriggerExit2D(Collider2D collision)
    {
        // if player descends through border
        if(collision.transform.position.y < gameObject.transform.position.y)
        {
            objectsUp.SetActive(false);
            objectsDown.SetActive(true);
        }
        else
        {
            // if player climbs through border
            objectsUp.SetActive(true);
            objectsDown.SetActive(false);
        }
    }
}
