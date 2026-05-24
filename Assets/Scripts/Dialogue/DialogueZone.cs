using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueZone : MonoBehaviour
{
    [SerializeField] private DialogueSequence dialogueSequence;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private GameObject playerPosition;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FadeTransition(collision.attachedRigidbody.gameObject);
        }
    }

    async void FadeTransition(GameObject player)
    {

        await FadeManager.Instance.FadeToBlack();

        await Task.Delay(500);

        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Dialogue");
        player.transform.position = playerPosition.transform.position;

        await FadeManager.Instance.FadeToTransparent();
        
        DialogueManager.Instance.StartDialogue(dialogueSequence);

        boxCollider.enabled = false;
    }
}
