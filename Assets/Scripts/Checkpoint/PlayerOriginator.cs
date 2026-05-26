using UnityEngine;

/// <summary>
/// ORIGINATOR (2D version)
/// Attached to the Player GameObject.
/// Knows how to pack its own state into a PlayerMemento and restore from one.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerOriginator : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private float _health  = 100f;
    [SerializeField] private float _stamina = 100f;
    [SerializeField] private int   _score   = 0;

    private Rigidbody2D _rb;

    public float Health  => _health;
    public float Stamina => _stamina;
    public int   Score   => _score;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("[Checkpoint] X pressed.");

            if (CheckpointManager.Instance == null)
            {
                Debug.LogError("[Checkpoint] CheckpointManager is NULL! Make sure it's in the scene.");
                return;
            }

            if (!CheckpointManager.Instance.HasCheckpoint())
            {
                Debug.LogWarning("[Checkpoint] X pressed but no checkpoint has been saved yet.");
                return;
            }

            Debug.Log("[Checkpoint] Restoring last checkpoint...");
            CheckpointManager.Instance.RestoreLastCheckpoint(this);
        }
    }

    // -------------------------------------------------------------------------
    // Memento pattern: Save
    // -------------------------------------------------------------------------

    public PlayerMemento SaveState()
    {
        return new PlayerMemento(
            position:        transform.position,
            rotation:        transform.rotation,
            velocity:        _rb.linearVelocity,
            angularVelocity: new Vector3(0f, 0f, _rb.angularVelocity),
            health:          _health,
            stamina:         _stamina,
            score:           _score
        );
    }

    // -------------------------------------------------------------------------
    // Memento pattern: Restore
    // -------------------------------------------------------------------------

    public void RestoreState(PlayerMemento memento)
    {
        if (memento == null)
        {
            Debug.LogWarning("[Checkpoint] No memento to restore from.");
            return;
        }

        _rb.bodyType = RigidbodyType2D.Kinematic;
        transform.SetPositionAndRotation(memento.Position, memento.Rotation);
        _rb.bodyType        = RigidbodyType2D.Dynamic;
        _rb.linearVelocity  = memento.Velocity;
        _rb.angularVelocity = memento.AngularVelocity.z;

        _health  = memento.Health;
        _stamina = memento.Stamina;
        _score   = memento.Score;

        Debug.Log($"[Checkpoint] Restored to {memento.Position} (saved at T={memento.TimeStamp:F1}s)");
    }

    // -------------------------------------------------------------------------
    // Save / Load — called by SaveTestUI
    // -------------------------------------------------------------------------

    /// <summary>
    /// Saves the current game state to disk with a given checkpoint name.
    /// Used for manual/UI-triggered saves.
    /// </summary>
    public void SaveGameState(string checkpointName)
    {
        if (CheckpointManager.Instance == null)
        {
            Debug.LogError("[SaveSystem] CheckpointManager not found!");
            return;
        }

        PlayerMemento memento = SaveState();

        SaveData data = new SaveData
        {
            PlayerX            = memento.Position.x,
            PlayerY            = memento.Position.y,
            RotationZ          = memento.Rotation.eulerAngles.z,
            Health             = memento.Health,
            Stamina            = memento.Stamina,
            Score              = memento.Score,
            LastCheckpointName = checkpointName,
        };

        SaveSystem.Save(data);
        Debug.Log($"[SaveSystem] Manual save at '{checkpointName}'.");
    }

    /// <summary>
    /// Loads the game state from disk and restores the full scene.
    /// </summary>
    public void LoadGameState()
    {
        if (CheckpointManager.Instance == null)
        {
            Debug.LogError("[SaveSystem] CheckpointManager not found!");
            return;
        }

        CheckpointManager.Instance.LoadGame();
    }

    // -------------------------------------------------------------------------
    // Stat mutators
    // -------------------------------------------------------------------------

    public void TakeDamage(float amount) => _health  = Mathf.Max(0f, _health  - amount);
    public void UseStamina(float amount) => _stamina = Mathf.Max(0f, _stamina - amount);
    public void AddScore(int amount)     => _score  += amount;
}