using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ally", menuName = "Combat/Entity/Ally")]
public class PlayerEntity : BaseEntity
{
    [field: SerializeField]
    public int MaxMana { get; private set; }
    [field: SerializeField]
    public int CurrentMana { get; private set; }
    
    public void RestoreStats()
    {
        RestoreHealth();
        CurrentMana = MaxMana;
    }

    public void ConsumeMana(int mana)
    {
        if (mana <= 0) return;
        
        CurrentMana -= mana;
        ToastsHandler.Instance.CreateToastMessage($"{Name} consumed {mana} mana!");
        
        if (CurrentMana < 0) CurrentMana = 0;
    }
    
    public void RestoreMana(int mana)
    {
        if (mana <= 0) return;
        
        var restoredMana = Math.Min(mana, MaxMana - CurrentMana);
        CurrentMana += restoredMana;
        ToastsHandler.Instance.CreateToastMessage($"{Name} restored {restoredMana} mana!");
    }
}