using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private PlayerMemento     _latestMemento;
    public  CheckpointTrigger LastCheckpoint { get; private set; }
    public  bool HasCheckpoint() => _latestMemento != null;

    private void Awake()
    {
        // If an old instance exists (from DontDestroyOnLoad in editor),
        // destroy the OLD one and replace it with this fresh one.
        // This ensures the latest compiled code is always running.
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (SaveSystem.HasSave())
            LoadGame();
    }

    public void SaveCheckpoint(PlayerOriginator player, CheckpointTrigger trigger)
    {
        if (player == null || trigger == null)
        {
            Debug.LogWarning("[Checkpoint] Save failed: missing player or trigger.");
            return;
        }

        // Destroy the collider on the previous checkpoint permanently
        if (LastCheckpoint != null)
        {
            BoxCollider2D prev = LastCheckpoint.GetComponent<BoxCollider2D>();
            if (prev != null)
            {
                Destroy(prev);
                Debug.Log("[Checkpoint] Destroyed BoxCollider2D on '" + LastCheckpoint.name + "'.");
            }
            else
            {
                Debug.LogError("[Checkpoint] '" + LastCheckpoint.name + "' has no BoxCollider2D to destroy!");
            }
        }
        else
        {
            Debug.Log("[Checkpoint] First checkpoint reached — no previous to destroy.");
        }

        _latestMemento = player.SaveState();
        LastCheckpoint  = trigger;
        Debug.Log("[Checkpoint] Active checkpoint is now '" + trigger.name + "'.");

        SaveGame();
    }

    public void RestoreLastCheckpoint(PlayerOriginator player)
    {
        if (_latestMemento == null) { Debug.LogWarning("[Checkpoint] No checkpoint saved yet."); return; }
        if (player == null) { Debug.LogWarning("[Checkpoint] Restore failed: player is null."); return; }
        player.RestoreState(_latestMemento);
    }

    public void SaveGame()
    {
        if (_latestMemento == null) { Debug.LogWarning("[SaveSystem] No memento to save."); return; }

        SaveData data = new SaveData
        {
            PlayerX            = _latestMemento.Position.x,
            PlayerY            = _latestMemento.Position.y,
            RotationZ          = _latestMemento.Rotation.eulerAngles.z,
            Health             = _latestMemento.Health,
            Stamina            = _latestMemento.Stamina,
            Score              = _latestMemento.Score,
            LastCheckpointName = LastCheckpoint != null ? LastCheckpoint.name : string.Empty,
        };
        SaveSystem.Save(data);
    }

    public void LoadGame()
    {
        SaveData data = SaveSystem.Load();
        if (data == null) return;

        foreach (var cp in FindObjectsByType<CheckpointTrigger>(FindObjectsSortMode.None))
        {
            if (cp.name == data.LastCheckpointName)
                LastCheckpoint = cp;
        }

        _latestMemento = new PlayerMemento(
            position:        new Vector3(data.PlayerX, data.PlayerY, 0f),
            rotation:        Quaternion.Euler(0f, 0f, data.RotationZ),
            velocity:        Vector3.zero,
            angularVelocity: Vector3.zero,
            health:          data.Health,
            stamina:         data.Stamina,
            score:           data.Score
        );

        PlayerOriginator player = FindFirstObjectByType<PlayerOriginator>();
        if (player == null) { Debug.LogError("[SaveSystem] PlayerOriginator not found!"); return; }
        player.RestoreState(_latestMemento);
        Debug.Log("[SaveSystem] Game loaded. Player at (" + data.PlayerX + ", " + data.PlayerY + ")");
    }

    private void OnApplicationQuit()
    {
#if UNITY_EDITOR
        SaveSystem.DeleteSave();
#endif
    }
}