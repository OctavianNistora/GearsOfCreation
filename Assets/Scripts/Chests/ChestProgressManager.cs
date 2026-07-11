using UnityEngine;

public class ChestProgressManager : MonoBehaviour
{
    public static ChestProgressManager Instance;

    public bool[] openedChests = new bool[3];

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetProgress()
    {
        openedChests = new bool[3];
    }
}
