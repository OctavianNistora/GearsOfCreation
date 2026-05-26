using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    public static EnemyFactory Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public GameObject CreateEnemy(EnemyData data, Vector3 position)
    {
        GameObject enemy = Instantiate(data.combatPrefab, position, Quaternion.identity);

        return enemy;
    }
}
