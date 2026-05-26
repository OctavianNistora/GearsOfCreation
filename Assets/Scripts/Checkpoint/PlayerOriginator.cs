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

    // Public read-only accessors so the UI / other systems can read stats
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

    /// <summary>
    /// Creates a memento capturing the player's current state.
    /// Called by CheckpointTrigger when the player enters a checkpoint.
    /// </summary>
    public PlayerMemento SaveState()
    {
        return new PlayerMemento(
            position:        transform.position,
            rotation:        transform.rotation,
            velocity:        _rb.linearVelocity,
            angularVelocity: new Vector3(0f, 0f, _rb.angularVelocity), // 2D: single float → Z axis
            health:          _health,
            stamina:         _stamina,
            score:           _score
        );
    }

    // -------------------------------------------------------------------------
    // Memento pattern: Restore
    // -------------------------------------------------------------------------

    /// <summary>
    /// Restores the player to the state captured in the given memento.
    /// Called by CheckpointManager when the player presses X.
    /// </summary>
    public void RestoreState(PlayerMemento memento)
    {
        if (memento == null)
        {
            Debug.LogWarning("PlayerOriginator: No memento to restore from.");
            return;
        }

        // Disable physics briefly to teleport cleanly
        _rb.bodyType = RigidbodyType2D.Kinematic;

        transform.SetPositionAndRotation(memento.Position, memento.Rotation);
        Debug.Log($"[Checkpoint] Transform set to {memento.Position}. Current pos after set: {transform.position}");

        _rb.bodyType        = RigidbodyType2D.Dynamic;
        _rb.linearVelocity  = memento.Velocity;
        _rb.angularVelocity = memento.AngularVelocity.z; // 2D: only Z matters

        // Stats
        _health  = memento.Health;
        _stamina = memento.Stamina;
        _score   = memento.Score;

        Debug.Log($"[Checkpoint] Restored to position {memento.Position} " +
                  $"(saved at T={memento.TimeStamp:F1}s)");
    }

    // -------------------------------------------------------------------------
    // Example stat mutators — replace/extend with your own game logic
    // -------------------------------------------------------------------------

    public void TakeDamage(float amount) => _health  = Mathf.Max(0f, _health  - amount);
    public void UseStamina(float amount) => _stamina = Mathf.Max(0f, _stamina - amount);
    public void AddScore(int amount)     => _score  += amount;
}