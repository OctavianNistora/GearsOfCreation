using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only react to the player
        PlayerOriginator player = other.GetComponent<PlayerOriginator>();
        if (player == null)
        {
            return;
        }

        // Hand the player to the caretaker to snapshot
        FadeToCheckpoint(player);
    }

    async void FadeToCheckpoint(PlayerOriginator player )
    {
        await FadeManager.Instance.FadeToBlack();
        
        CheckpointManager.Instance.RestoreLastCheckpoint(player);

        await FadeManager.Instance.FadeToTransparent();
    }
}


