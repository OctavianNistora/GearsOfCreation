using System.Collections.Generic;
using DefaultNamespace.Combat.Items;
using UnityEngine;

public class PartyInitializer : MonoBehaviour
{
    void Start()
    {
        var mainCharacter = new PlayerEntity(
            "Hero", 
            100,
            100,
            50,
            50,
            new List<BaseCharacterAbility>()
            {
                new SingleAttack("Stab", 25, 0),
                new AoeAttack("Slash", 15, 5),
            }
            );
        PartyManager.Instance.Members.Add(mainCharacter);

        var sideCharacter = new PlayerEntity(
            "Side Hero",
            100,
            100,
            50,
            50,
            new List<BaseCharacterAbility>()
            {
                new SingleAttack("Stab", 20, 0),
                new Heal("Heal", 40, 10),
            }
            );
        PartyManager.Instance.Members.Add(sideCharacter);
        
        
        var smallPotion = new HealthPotion("Small Health Potion", 3, 25);
        PartyManager.Instance.Inventory.Add(smallPotion);
        
        var mediumPotion = new HealthPotion("Medium Health Potion", 1, 50);
        PartyManager.Instance.Inventory.Add(mediumPotion);
        
        var smallStrengthPotion = new StrengthPotion("Small Strength Potion", 2, 3, 1.25f);
        PartyManager.Instance.Inventory.Add(smallStrengthPotion);
    }
}
