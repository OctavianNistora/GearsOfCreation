using UnityEngine;

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
        
        CheckpointManager.Instance.LoadGame();
    }

    // X key handling removed from here — Pause.cs owns it now

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

    public void RestoreState(PlayerMemento memento)
    {
        if (memento == null) { Debug.LogWarning("[Checkpoint] No memento to restore from."); return; }

        _rb.bodyType        = RigidbodyType2D.Kinematic;
        transform.SetPositionAndRotation(memento.Position, memento.Rotation);
        _rb.bodyType        = RigidbodyType2D.Dynamic;
        _rb.linearVelocity  = memento.Velocity;
        _rb.angularVelocity = memento.AngularVelocity.z;

        _health  = memento.Health;
        _stamina = memento.Stamina;
        _score   = memento.Score;

        Debug.Log("[Checkpoint] Restored to " + memento.Position);
    }

    public void SaveGameState(string checkpointName)
    {
        if (CheckpointManager.Instance == null) { Debug.LogError("[SaveSystem] CheckpointManager not found!"); return; }

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
        Debug.Log("[SaveSystem] Manual save at '" + checkpointName + "'.");
    }

    public void LoadGameState()
    {
        if (CheckpointManager.Instance == null) { Debug.LogError("[SaveSystem] CheckpointManager not found!"); return; }
        CheckpointManager.Instance.LoadGame();
    }

    public void TakeDamage(float amount) => _health  = Mathf.Max(0f, _health  - amount);
    public void UseStamina(float amount) => _stamina = Mathf.Max(0f, _stamina - amount);
    public void AddScore(int amount)     => _score  += amount;
}