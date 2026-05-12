using System.Collections.Generic;
using DefaultNamespace.Combat.Items;
using UnityEngine;

public class PartyInitializer : MonoBehaviour
{
    [SerializeField] private List<PlayerEntity> initialPartyMembers;
    [SerializeField] private List<BaseCombatItem> initialInventoryItems;
    
    void Start()
    {
        initialPartyMembers.ForEach(member =>
        {
            var instance = (PlayerEntity)member.CreateInstance();
            PartyManager.Instance.Members.Add(instance);
        });
        initialInventoryItems.ForEach(item =>
        {
            var instance = (BaseCombatItem)item.CreateInstance();
            instance.IncreaseQuantity(1);
            PartyManager.Instance.Inventory.Add(instance);
        });
        
        /*var mainCharacter = new PlayerEntity(
            "Hero", 
            100,
            100,
            50,
            50,
            new List<BaseCharacterAbility>()
            {
                new SingleAttack("Slash", 0, 1, 25),
                new AoeAttack("Cleave", 5, 3, 15),
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
                new SingleAttack("Slash", 0, 1, 20),
                new Heal("Heal", 10, 2, 40),
            }
            );
        PartyManager.Instance.Members.Add(sideCharacter);
        
        
        var smallPotion = new HealthPotion("Small Health Potion", 3, 25, 2);
        PartyManager.Instance.Inventory.Add(smallPotion);
        
        var mediumPotion = new HealthPotion("Medium Health Potion", 1, 50, 2);
        PartyManager.Instance.Inventory.Add(mediumPotion);
        
        var smallStrengthPotion = new StrengthPotion("Small Strength Potion", 2, 3, 1.25f, 0);
        PartyManager.Instance.Inventory.Add(smallStrengthPotion);*/
    }
}
