using UnityEngine;

public class EnemyPlacer : MonoBehaviour
{
    [SerializeField] GameObject enemyPositionsParent;
    private Vector3[] enemyPositions = new Vector3[3];

    void Awake()
    {
        int enemyPosCounter = 0;
        foreach (Transform childTransform in enemyPositionsParent.transform)
        {
            enemyPositions[enemyPosCounter++] = childTransform.position;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int enemyPosCounter = 0;
        foreach (var enemyData in EncounterManager.Instance.enemiesToSpawn)
        {
            EnemyFactory.Instance.CreateEnemy(enemyData, enemyPositions[enemyPosCounter++]);
        }
    }
}
