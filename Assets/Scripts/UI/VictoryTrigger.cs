using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;

public class VictoryTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.attachedRigidbody.gameObject;
            player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Dialogue");
            VictoryScreen.Instance.children.SetActive(true);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.victory);
        }
    }
}
