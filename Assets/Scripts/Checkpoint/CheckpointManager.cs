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

/// CARETAKER — extended with save/load.
/// - Checkpoint reached  → snapshots memento + writes to disk.
/// - Game start          → loads from disk if a save exists.
/// </summary>
public class CheckpointManager : MonoBehaviour
{
/// CARETAKER — extended with save/load.
/// - Checkpoint reached  → snapshots memento + writes to disk.
/// - Game start          → loads from disk if a save exists.
/// </summary>
public class CheckpointManager : MonoBehaviour
{
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
        DontDestroyOnLoad(gameObject);
    }

    private PlayerMemento _latestMemento;

    public CheckpointTrigger LastCheckpoint { get; private set; }

    public bool HasCheckpoint() => _latestMemento != null;

    // Runs after the scene is fully loaded
    private void Start()
    {
        if (SaveSystem.HasSave())
            LoadGame();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CHECKPOINT REACHED
    // ─────────────────────────────────────────────────────────────────────────

    public void SaveCheckpoint(PlayerOriginator player, CheckpointTrigger trigger)
    {
        // Disable ONLY the previous checkpoint collider
        if (LastCheckpoint != null)
        {
            Collider2D previousCollider = LastCheckpoint.GetComponent<Collider2D>();

            if (previousCollider != null)
            {
                previousCollider.enabled = false;

                Debug.Log(
                    $"[Checkpoint] Disabled collider on '{LastCheckpoint.name}'.");
            }
        }

        // Save new checkpoint
        _latestMemento = player.SaveState();
        LastCheckpoint = trigger;

        Debug.Log(
            $"[Checkpoint] Reached '{trigger.name}' pos={_latestMemento.Position}");

        // Write save to disk
        SaveGame();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // RESTORE IN CURRENT SESSION
    // ─────────────────────────────────────────────────────────────────────────

    public void RestoreLastCheckpoint(PlayerOriginator player)
    {
        if (_latestMemento == null)
        {
            Debug.LogWarning("[Checkpoint] No checkpoint saved yet.");
            Debug.LogWarning("[Checkpoint] Nothing saved yet.");
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

    // ─────────────────────────────────────────────────────────────────────────
    // SAVE TO DISK
    // ─────────────────────────────────────────────────────────────────────────

    public void SaveGame()
    {
        if (_latestMemento == null)
        {
            Debug.LogWarning("[SaveSystem] No memento to save.");
            return;
        }

        SaveData data = new SaveData
        {
            PlayerX = _latestMemento.Position.x,
            PlayerY = _latestMemento.Position.y,

            RotationZ = _latestMemento.Rotation.eulerAngles.z,

            Health = _latestMemento.Health,
            Stamina = _latestMemento.Stamina,
            Score = _latestMemento.Score,

            LastCheckpointName =
                LastCheckpoint != null
                    ? LastCheckpoint.name
                    : string.Empty,
        };

        SaveSystem.Save(data);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // LOAD FROM DISK
    // ─────────────────────────────────────────────────────────────────────────

    public void LoadGame()
    {
        SaveData data = SaveSystem.Load();

        if (data == null)
            return;

        // Re-enable ALL checkpoint colliders first
        // Then remember which one was last reached
        foreach (var cp in FindObjectsByType<CheckpointTrigger>(
                     FindObjectsSortMode.None))
        {
            Collider2D col = cp.GetComponent<Collider2D>();

            if (col != null)
                col.enabled = true;

            // Restore reference to the saved checkpoint
            if (cp.name == data.LastCheckpointName)
                LastCheckpoint = cp;
        }

        // Rebuild memento from save data
        _latestMemento = new PlayerMemento(
            position: new Vector3(data.PlayerX, data.PlayerY, 0f),

            rotation: Quaternion.Euler(0f, 0f, data.RotationZ),

            velocity: Vector3.zero,
            angularVelocity: Vector3.zero,

            health: data.Health,
            stamina: data.Stamina,
            score: data.Score
        );

        // Restore player
        PlayerOriginator player =
            FindFirstObjectByType<PlayerOriginator>();

        if (player == null)
        {
            Debug.LogError(
                "[SaveSystem] PlayerOriginator not found in scene!");
            return;
        }

        player.RestoreState(_latestMemento);

        Debug.Log(
            $"[SaveSystem] Loaded. Player → ({data.PlayerX:F1}, {data.PlayerY:F1}) " +
            $"HP={data.Health} checkpoint='{data.LastCheckpointName}'");
    }

    private void OnApplicationQuit()
    {
        #if UNITY_EDITOR
            SaveSystem.DeleteSave();
        #endif
    }
}