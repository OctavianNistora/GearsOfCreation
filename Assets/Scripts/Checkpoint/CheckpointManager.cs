using UnityEngine;

/// <summary>
/// CARETAKER
/// Singleton that stores the latest PlayerMemento.
/// CheckpointTriggers push new mementos here;
/// PlayerOriginator pulls from here when restoring.
/// </summary>
public class CheckpointManager : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Singleton
    // -------------------------------------------------------------------------

    public static CheckpointManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Remove if you don't need cross-scene persistence
    }

    // -------------------------------------------------------------------------
    // Caretaker state
    // -------------------------------------------------------------------------

    private PlayerMemento _latestMemento;

    /// <summary>The checkpoint trigger that produced the current memento.</summary>
    public CheckpointTrigger LastCheckpoint { get; private set; }

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    /// <summary>
    /// Called by a CheckpointTrigger to store the player's state.
    /// </summary>
    public void SaveCheckpoint(PlayerOriginator player, CheckpointTrigger trigger)
    {
        // Disable the collider on the previous checkpoint so it can't trigger again
        if (LastCheckpoint != null)
        {
            Collider2D prevCollider = LastCheckpoint.GetComponent<Collider2D>();
            if (prevCollider != null)
                prevCollider.enabled = false;

            Debug.Log($"[Checkpoint] Disabled collider on previous checkpoint '{LastCheckpoint.name}'.");
        }

        _latestMemento = player.SaveState();
        LastCheckpoint = trigger;

        Debug.Log($"[Checkpoint] Saved at '{trigger.name}' " +
                  $"pos={_latestMemento.Position} HP={_latestMemento.Health}");
    }

    /// <summary>
    /// Called by PlayerOriginator (X key) to restore the last saved state.
    /// </summary>
    public void RestoreLastCheckpoint(PlayerOriginator player)
    {
        if (_latestMemento == null)
        {
            Debug.LogWarning("[Checkpoint] No checkpoint saved yet.");
            return;
        }

        player.RestoreState(_latestMemento);
    }

    /// <summary>
    /// Returns true if at least one checkpoint has been saved.
    /// Useful for UI ("Press X to respawn" hint).
    /// </summary>
    public bool HasCheckpoint() => _latestMemento != null;
}