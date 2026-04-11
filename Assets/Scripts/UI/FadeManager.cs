using System.Threading.Tasks;
using UnityEngine;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    async Task Fade(float targetTransparency)
    {
        float start = canvasGroup.alpha;
        float currentTime = 0;
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, targetTransparency, currentTime/fadeDuration);
            await Task.Yield();
        }
        canvasGroup.alpha = targetTransparency;
    }

    public async Task FadeToBlack()
    {
        await Fade(1); 
    }

    public async Task FadeToTransparent()
    {
        await Fade(0); 
    }
}
