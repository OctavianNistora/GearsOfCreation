using System.Collections.Generic;
using UnityEngine;

public class EncounterInitializer : MonoBehaviour
{
    [SerializeField] private List<EnemyEntity> initialPartyMembers;
    
    void Start()
    {
        initialPartyMembers.ForEach(member =>
        {
            {
                var instance = (EnemyEntity)member.CreateInstance();
                CombatManager.Instance.enemies.Add(instance);
            }
        });
        
        /*var firstEnemy = new EnemyEntity(
            "Thief", 
            100,
            100,
            new List<BaseCharacterAbility>()
            {
                new SingleAttack("Slash", 20, 0, 1),
            }
        );
        CombatManager.Instance.enemies.Add(firstEnemy);

        var secondEnemy = new EnemyEntity(
            "Sentient Tree",
            100,
            100,
            new List<BaseCharacterAbility>()
            {
                new AoeAttack("Cleave", 10, 0, 3),
            }
        );
        CombatManager.Instance.enemies.Add(secondEnemy);*/
    }
}
