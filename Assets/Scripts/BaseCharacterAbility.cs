using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCharacterAbility : BaseAction
{
    [field: SerializeField]
    public int ManaCost { get; private set; }
    
    public override IEnumerator Apply(BaseEntity source, List<BaseEntity> targets)
    {
        if (source is PlayerEntity player)
        {
            if (player.CurrentMana < ManaCost)
            {
                ToastsHandler.Instance.CreateToastMessage($"{player.Name} does not have enough mana to use {Name}!");
                yield break;
            }
            
            player.ConsumeMana(ManaCost);
        }
        
        yield return base.Apply(source, targets);
    }
}

public abstract class DamagingCharacterAbility : BaseCharacterAbility
{
    [field: SerializeField]
    public int Damage { get; private set; }
}

public abstract class HealingCharacterAbility : BaseCharacterAbility
{
    [field: SerializeField]
    public int HealAmount { get; private set; }
}