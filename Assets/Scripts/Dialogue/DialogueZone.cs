using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueZone : MonoBehaviour
{
    [SerializeField] private DialogueSequence dialogueSequence;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FadeTransition(collision.attachedRigidbody.gameObject);
        }
    }

    async void FadeTransition(GameObject player)
    {
        // player input might be disabled/enabled in a dialogue manager
        player.GetComponent<PlayerInput>().actions.Disable();

        await FadeManager.Instance.FadeToBlack();

        player.transform.position = transform.position;

        await FadeManager.Instance.FadeToTransparent();
        
        //player.GetComponent<PlayerInput>().actions.Enable();

        //Destroy(gameObject);

        DialogueManager.Instance.StartDialogue(dialogueSequence);
    }
}
