using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    
    [Header("Background music")]
    public AudioClip mainMenu;
    public AudioClip tutorial;
    public AudioClip glassyWaters;
    public AudioClip combat;

    [Header("Player")]
    public AudioClip jump;
    public AudioClip walk;
    public AudioClip defeat;
    public AudioClip victory;

    [Header("Ability SFX")]
    public AudioClip slash;
    public AudioClip cleave;
    public AudioClip heal;
    public AudioClip rain;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}