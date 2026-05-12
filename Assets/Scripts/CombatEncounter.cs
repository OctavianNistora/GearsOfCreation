using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Combat Encounter", menuName = "Combat/Combat Encounter")]
public class CombatEncounter : ScriptableObject
{
    public List<EnemyEntity> enemies;
}
