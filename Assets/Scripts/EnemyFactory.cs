using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    public static EnemyFactory Instance;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject CreateEnemy(EnemyData data, Vector3 position)
    {
        GameObject enemy = Instantiate(data.combatPrefab, position, Quaternion.identity);

        return enemy;
    }
}
