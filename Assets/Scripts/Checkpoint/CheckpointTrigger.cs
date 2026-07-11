using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("If true, this checkpoint can only be activated once.")]
    [SerializeField] private bool _activateOnce = true; // default true — checkpoint should only save once

    [SerializeField] private SpriteRenderer _checkpointSpriteRenderer;
    [SerializeField] private Sprite _inactiveSprite;
    [SerializeField] private Sprite _activeSprite;

    public int checkpointIndex; // index to identify this checkpoint in the CheckpointManager

    private bool _hasBeenActivated = false;

    private void Start()
    {
        if (CheckpointManager.Instance.activeCheckpoints[checkpointIndex])
        {
            _hasBeenActivated = true;
            UpdateVisual(active: true);
        }
        else
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
        CheckpointManager.Instance.activeCheckpoints[checkpointIndex] = true;
        UpdateVisual(active: true);
    }

    private void UpdateVisual(bool active)
    {
        _checkpointSpriteRenderer.sprite = active ? _activeSprite : _inactiveSprite;
    }

    public void Deactivate()
    {
        _hasBeenActivated = false;
        UpdateVisual(active: false);
    }
}