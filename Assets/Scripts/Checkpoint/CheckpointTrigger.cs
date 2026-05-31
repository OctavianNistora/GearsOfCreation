using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("If true, this checkpoint can only be activated once.")]
    [SerializeField] private bool _activateOnce = true; // default true — checkpoint should only save once

    [Header("Visual Feedback (optional)")]
    [SerializeField] private Renderer _checkpointRenderer;
    [SerializeField] private Color _inactiveColor = Color.white;
    [SerializeField] private Color _activeColor   = Color.green;

    private bool _hasBeenActivated = false;

    private void Start()
    {
        UpdateVisual(active: false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Already activated and set to once-only — ignore
        if (_activateOnce && _hasBeenActivated) return;

        // Already the active checkpoint — ignore
        if (CheckpointManager.Instance.LastCheckpoint == this) return;

        PlayerOriginator player = other.GetComponent<PlayerOriginator>();
        if (player == null) return;

        Debug.Log("[Checkpoint] Reached: '" + name + "'");

        CheckpointManager.Instance.SaveCheckpoint(player, this);

        _hasBeenActivated = true;
        UpdateVisual(active: true);
    }

    private void UpdateVisual(bool active)
    {
        if (_checkpointRenderer == null) return;
        _checkpointRenderer.material.color = active ? _activeColor : _inactiveColor;
    }

    public void Deactivate()
    {
        _hasBeenActivated = false;
        UpdateVisual(active: false);
    }
}