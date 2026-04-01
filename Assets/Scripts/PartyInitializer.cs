using System.Collections.Generic;
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
            new List<BaseAbility>()
            {
                new SingleAttack("Stab", 25),
                new AoeAttack("Slash", 15),
            }
            );
        PartyManager.Instance.members.Add(mainCharacter);

        var sideCharacter = new PlayerEntity(
            "Side Hero",
            100,
            100,
            50,
            50,
            new List<BaseAbility>()
            {
                new SingleAttack("Stab", 20),
                new Heal("Heal", 40),
            }
            );
        PartyManager.Instance.members.Add(sideCharacter);
    }
}
