using System.Collections.Generic;
using UnityEngine;

public class GroupCombatCharactersHandler : MonoBehaviour
{
    [SerializeField]
    private List<CombatCharacterAnimationHandler> allies = new();
    [SerializeField]
    private List<CombatCharacterAnimationHandler> enemies = new();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < allies.Count; i++)
        {
            if (i >= PartyManager.Instance.Members.Count)
            {
                Destroy(allies[i].gameObject);
                continue;
            }

            allies[i].Initialize();
            PartyManager.Instance.Members[i].CombatCharacterAnimationHandler = allies[i];
        }

        var enemyOffset = 0;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies.Count - i > CombatManager.Instance.enemies.Count)
            {
                Destroy(enemies[i].gameObject);
                enemyOffset++;
                continue;
            }
            
            enemies[i].Initialize();
            CombatManager.Instance.enemies[i-enemyOffset].CombatCharacterAnimationHandler = enemies[i];
        }
    }
}
