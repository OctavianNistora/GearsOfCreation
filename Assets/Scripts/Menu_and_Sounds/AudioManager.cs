using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("----------Audio Source----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource walkSource;

    [Header("----------Audio Clip----------")]
    public AudioClip background;
    public AudioClip jump;
    public AudioClip death;
    public AudioClip walk;

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

    private void Start()
    {
        musicSource.clip = background;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PlayWalk()
    {
        if (walkSource.isPlaying) return;

        walkSource.clip = walk;
        walkSource.loop = true;
        walkSource.Play();
    }

    public void StopWalk()
    {
        if (!walkSource.isPlaying) return;

        walkSource.Stop();
    }
}