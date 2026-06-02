using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.U2D.Animation;

public abstract class BaseEntity : ScriptableObject
{
    [field: SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    public int MaxHp { get; private set; }
    [field: SerializeField]
    public int CurrentHp { get; private set; }
    [field: SerializeField]
    public List<BaseCharacterAbility> Abilities { get; private set; }
    [field: SerializeField]
    public Sprite Thumbnail { get; private set; }
    [field: SerializeField]
    public SpriteLibraryAsset characterSpriteLibraryAsset { get; private set; }
    public CombatCharacterAnimationHandler CombatCharacterAnimationHandler { get; set; }

    private List<BaseCombatModifier> _modifiers = new List<BaseCombatModifier>();

    public event Action OnDamageTaken;
    public event Action OnHeal;
    public event Action OnDeath;

    public IEnumerator Damage(int damage)
    {
        if (CurrentHp <= 0) yield break;
        
        var damageModifiers = _modifiers.OfType<DamageReceivedModifier>().ToList();
        
        var modifiedDamage = damageModifiers.Aggregate((float)damage, (current, modifier) => modifier.Apply(current));
        
        CurrentHp -= (int)modifiedDamage;
        ToastsHandler.Instance.CreateToastMessage($"{Name} took {modifiedDamage} damage!");
        yield return CombatCharacterAnimationHandler.PlayHurtAnimation();
        OnDamageTaken?.Invoke();
        
        if (CurrentHp > 0) yield break;
        
        CurrentHp = 0;
        ToastsHandler.Instance.CreateToastMessage($"{Name} has died!");
        yield return CombatCharacterAnimationHandler.PlayDeathAnimation();
        OnDeath?.Invoke();
    }
    
    public void Heal(int heal)
    {
        if (CurrentHp <= 0) return;
        
        var healedAmount = Math.Min(heal, MaxHp - CurrentHp);
        CurrentHp += healedAmount;
        ToastsHandler.Instance.CreateToastMessage($"{Name} was healed for {healedAmount} hp!");
        OnHeal?.Invoke();
        
    }

    public void RestoreHealth()
    {
        CurrentHp = MaxHp;
    }
    
    public void AddModifier(BaseCombatModifier modifier)
    {
        _modifiers.Add(modifier);
    }
    
    public void RemoveModifier(BaseCombatModifier modifier)
    {
        _modifiers.Remove(modifier);
    }

    public virtual BaseEntity CreateInstance()
    {
        var instance = Instantiate(this);
        instance.Abilities = Abilities.Select(ability => Instantiate(ability)).ToList();
        return instance;
    }
}