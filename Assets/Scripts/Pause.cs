using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public static Pause Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject _pauseCanvas;

    // Eliminat [SerializeField] pentru _playerInput deoarece va fi căutat dinamic
    private PlayerInput _cachedPlayerInput;
    private bool _isPaused = false;

    private void Awake()
    {
        // Logica de Singleton corectă pentru elemente persistente (DontDestroyOnLoad)
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject); // Face ca prefab-ul să supraviețuiască între scene
        
        _pauseCanvas.SetActive(false);
    }

    private void Update()
    {
        // Verificăm tasta X. Va funcționa în orice scenă
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

        // Căutăm dinamic PlayerInput-ul din scena curentă (dacă există)
        if (_cachedPlayerInput == null)
        {
            _cachedPlayerInput = FindFirstObjectByType<PlayerInput>();
        }

        // Dacă am găsit un jucător în scenă, îi gestionăm input-ul și fizica
        if (_cachedPlayerInput != null)
        {
            _cachedPlayerInput.enabled = !_isPaused;

            if (_isPaused)
            {
                // Oprim complet mișcarea fizică a jucătorului când e pauză
                Rigidbody2D rb = _cachedPlayerInput.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                }
            }
        }
        
        // Dacă dăm unpause, resetăm referința memorată pentru a o putea căuta proaspătă tura următoare 
        // (în caz că am schimbat scena între timp)
        if (!_isPaused)
        {
            _cachedPlayerInput = null;
        }
    }

    public void ResumeFromButton()
    {
        if (_isPaused) TogglePause();
    }

    public void QuitToMainMenu()
    {
        _isPaused = false;
        _pauseCanvas.SetActive(false);
        Time.timeScale = 1f;
        _cachedPlayerInput = null;
        
        SceneManager.LoadScene("Main_Menu");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Time.timeScale = 1f;
        }
    }
}