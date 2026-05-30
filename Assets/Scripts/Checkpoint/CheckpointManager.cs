using UnityEngine;

/// <summary>
/// CARETAKER
/// Manages player checkpoints using the Memento pattern.
/// Stores the latest saved PlayerMemento and restores it when requested.
/// </summary>
public class CheckpointManager : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Singleton Instance
    // -------------------------------------------------------------------------

    public static CheckpointManager Instance { get; private set; }

    // -------------------------------------------------------------------------
    // Private Fields
    // -------------------------------------------------------------------------

    private PlayerMemento latestMemento;

    // -------------------------------------------------------------------------
    // Public Properties
    // -------------------------------------------------------------------------

    /// <summary>
    /// The last checkpoint trigger that saved the player state.
    /// </summary>
    public CheckpointTrigger LastCheckpoint { get; private set; }

    // -------------------------------------------------------------------------
    // Unity Methods
    // -------------------------------------------------------------------------

    private void Awake()
    {
        // Singleton protection
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keeps the manager alive between scene loads
        DontDestroyOnLoad(gameObject);
    }

    // -------------------------------------------------------------------------
    // Save Checkpoint
    // -------------------------------------------------------------------------

    /// <summary>
    /// Saves the player's current state.
    /// Called by a checkpoint trigger.
    /// </summary>
    /// <param name="player">Player originator</param>
    /// <param name="trigger">Checkpoint trigger</param>
    public void SaveCheckpoint(PlayerOriginator player, CheckpointTrigger trigger)
    {
        if (player == null || trigger == null)
        {
            Debug.LogWarning("Checkpoint save failed: Missing player or trigger.");
            return;
        }

        latestMemento = player.SaveState();
        LastCheckpoint = trigger;

        Debug.Log(
            $"Checkpoint saved at '{trigger.name}' | " +
            $"Position: {latestMemento.Position} | " +
            $"Health: {latestMemento.Health}"
        );
    }

    // -------------------------------------------------------------------------
    // Restore Checkpoint
    // -------------------------------------------------------------------------

    /// <summary>
    /// Restores the player to the latest saved checkpoint.
    /// </summary>
    /// <param name="player">Player originator</param>
    public void RestoreLastCheckpoint(PlayerOriginator player)
    {
        if (latestMemento == null)
        {
            Debug.LogWarning("No checkpoint has been saved yet.");
            return;
        }

        if (player == null)
        {
            Debug.LogWarning("Restore failed: Player reference is missing.");
            return;
        }

        player.RestoreState(latestMemento);
    }

    // -------------------------------------------------------------------------
    // Utility
    // -------------------------------------------------------------------------

    /// <summary>
    /// Returns true if a checkpoint exists.
    /// </summary>
    public bool HasCheckpoint()
    {
        return latestMemento != null;
    }
}