using UnityEngine;

public class TutorialStepZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlatformerTutorial parent = gameObject.transform.parent.GetComponent<PlatformerTutorial>();
            parent.ShowStep();
            Destroy(gameObject);
        }
    }
}
