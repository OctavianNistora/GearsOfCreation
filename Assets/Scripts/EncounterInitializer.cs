using System.Collections.Generic;
using UnityEngine;

public class EncounterInitializer : MonoBehaviour
{
    void Start()
    {
        var firstEnemy = new EnemyEntity(
            "Thief", 
            100,
            100,
            new List<BaseAbility>()
            {
                new SingleAttack("Stab", 20),
            }
        );
        CombatManager.Instance.enemies.Add(firstEnemy);

        var secondEnemy = new EnemyEntity(
            "Sentient Tree",
            100,
            100,
            new List<BaseAbility>()
            {
                new AoeAttack("Slash", 10),
            }
        );
        CombatManager.Instance.enemies.Add(secondEnemy);
    }
}
