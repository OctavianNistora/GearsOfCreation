using UnityEngine;

/// <summary>
/// Place this on a GameObject that has a Collider set to "Is Trigger".
/// When the player walks through it, the current player state is saved.
/// </summary>
public class CheckpointTrigger : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("If true, this checkpoint can only be activated once.")]
    [SerializeField] private bool _activateOnce = false;

    [Header("Visual Feedback (optional)")]
    [SerializeField] private Renderer _checkpointRenderer;
    [SerializeField] private Color _inactiveColor = Color.white;
    [SerializeField] private Color _activeColor   = Color.green;

    private bool _hasBeenActivated = false;

    // -------------------------------------------------------------------------
    // Trigger
    // -------------------------------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Checkpoint] '{name}' trigger hit by: {other.name}");

        if (_activateOnce && _hasBeenActivated) return;

        // Only react to the player
        PlayerOriginator player = other.GetComponent<PlayerOriginator>();
        if (player == null)
        {
            Debug.LogWarning($"[Checkpoint] '{name}' was hit by '{other.name}' but it has no PlayerOriginator — ignoring.");
            return;
        }

        Debug.Log($"[Checkpoint] Checkpoint reached by '{other.name}'!");

        // Don't re-save if this is already the active checkpoint
        if (CheckpointManager.Instance.LastCheckpoint == this) return;

        // Hand the player to the caretaker to snapshot
        CheckpointManager.Instance.SaveCheckpoint(player, this);

        _hasBeenActivated = true;
        UpdateVisual(active: true);
    }

    // -------------------------------------------------------------------------
    // Visual helpers
    // -------------------------------------------------------------------------

    private void Start()
    {
        UpdateVisual(active: false);
    }

    private void UpdateVisual(bool active)
    {
        if (_checkpointRenderer == null) return;
        _checkpointRenderer.material.color = active ? _activeColor : _inactiveColor;
    }

    /// <summary>
    /// Called by CheckpointManager if a newer checkpoint is activated,
    /// so this one can go back to its inactive visual.
    /// Uncomment the call in CheckpointManager.SaveCheckpoint if you want this.
    /// </summary>
    public void Deactivate()
    {
        _hasBeenActivated = false;
        UpdateVisual(active: false);
    }
}