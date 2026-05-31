using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public static Pause Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject  _pauseCanvas;
    [SerializeField] private PlayerInput _playerInput;

    private bool _isPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        _pauseCanvas.SetActive(false);
    }

    private void Update()
    {
        // Time.unscaledDeltaTime based input check — works even when timeScale = 0
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("[Pause] X detected in Pause.Update");
            TogglePause();
        }
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;
        Debug.Log("[Pause] TogglePause -> isPaused=" + _isPaused);

        _pauseCanvas.SetActive(_isPaused);
        Time.timeScale = _isPaused ? 0f : 1f;
        _playerInput.enabled = !_isPaused;

        // Zero velocity so player doesn't drift while frozen
        Rigidbody2D rb = _playerInput.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity  = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    public void ResumeFromButton()
    {
        if (_isPaused) TogglePause();
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main_Menu");
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}