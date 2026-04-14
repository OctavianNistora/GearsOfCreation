using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance { get; private set; }
    
    public List<PlayerEntity> Members { get; private set; } = new();
    public List<BaseCombatItem> Inventory { get; private set; } = new();
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

public abstract class BaseEntity
{
    public string Name { get; private set; }
    public int MaxHp { get; private set; }
    public int CurrentHp { get; private set; }
    public List<BaseCharacterAbility> Abilities { get; private set; }
    
    private List<BaseCombatModifier> _modifiers = new();

    public event Action OnDamageTaken;
    public event Action OnHeal;
    public event Action OnDeath;

    protected BaseEntity(string name, int maxHp, int currentHp, List<BaseCharacterAbility> abilities)
    {
        Name = name;
        MaxHp = maxHp;
        CurrentHp = currentHp;
        Abilities = abilities;
    }
    
    public void Damage(int damage)
    {
        if (CurrentHp <= 0) return;
        
        var damageModifiers = _modifiers.OfType<DamageReceivedModifier>().ToList();
        
        var modifiedDamage = damageModifiers.Aggregate((float)damage, (current, modifier) => modifier.Apply(current));
        
        CurrentHp -= (int)modifiedDamage;
        ToastsHandler.Instance.CreateToastMessage($"{Name} took {modifiedDamage} damage!");
        OnDamageTaken?.Invoke();
        
        if (CurrentHp > 0) return;
        CurrentHp = 0;
        ToastsHandler.Instance.CreateToastMessage($"{Name} has died!");
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
    
    public void AddModifier(BaseCombatModifier modifier)
    {
        _modifiers.Add(modifier);
    }
    
    public void RemoveModifier(BaseCombatModifier modifier)
    {
        _modifiers.Remove(modifier);
    }
}

public class EnemyEntity : BaseEntity
{
    public EnemyEntity(string name, int maxHp, int currentHp, List<BaseCharacterAbility> abilities) : base(name, maxHp, currentHp, abilities)
    {
    }
}

public class PlayerEntity : BaseEntity
{
    public PlayerEntity(string name, int maxHp, int currentHp, int maxMana, int currentMana, List<BaseCharacterAbility> abilities) : base(name, maxHp, currentHp, abilities)
    {
        MaxMana = maxMana;
        CurrentMana = currentMana;
    }

    public int MaxMana { get; private set; }
    public int CurrentMana { get; private set; }
    
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

public abstract class BaseAction
{
    public string Name { get; private set; }
    public int TargetCount {get; private set;}
    public bool TargetEnemy  { get; private set; }
    
    protected BaseAction(string name, int targetCount, bool targetEnemy)
    {
        Name = name;
        TargetCount = targetCount;
        TargetEnemy = targetEnemy;
    }

    public virtual IEnumerator Apply(BaseEntity source, List<BaseEntity> targets)
    {
        var toastString = TargetCount > 0 && targets.Count > 0 ?
            $"{source.Name} used {Name} on {string.Join(", ", targets.Select(t => t.Name))}!" : 
            $"{source.Name} used {Name}!";
        ToastsHandler.Instance.CreateToastMessage(toastString);
        yield return new WaitForSeconds(1.5f);
        
        yield return ApplyLogic(source, targets);
    }
    
    protected abstract IEnumerator ApplyLogic(BaseEntity source, List<BaseEntity> targets);
}

public abstract class BaseCharacterAbility : BaseAction
{
    public int ManaCost { get; private set; }
    
    protected BaseCharacterAbility(string name, int targetCount, bool targetEnemy, int manaCost) : base(name, targetCount, targetEnemy)
    {
        ManaCost = manaCost;
    }
    
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

public class SingleAttack : BaseCharacterAbility
{
    public int Damage { get; private set; }
    
    public SingleAttack(string name, int damage, int manaCost) : base(name, 1, true, manaCost)
    {
        Damage = damage;
    }

    protected override IEnumerator ApplyLogic(BaseEntity source, List<BaseEntity> targets)
    {
        targets.First().Damage(Damage);
        yield break;
    }
}

public class AoeAttack : BaseCharacterAbility
{
    public int Damage { get; private set; }
    
    public AoeAttack(string name, int damage, int manaCost) : base(name, 0, true, manaCost)
    {
        Damage = damage;
    }

    protected override IEnumerator ApplyLogic(BaseEntity source, List<BaseEntity> targets)
    {
        targets.ForEach(target => target.Damage(Damage));
        yield break;
    }
}

public class Heal : BaseCharacterAbility
{
    public int HealAmount { get; private set; }
    
    public Heal(string name, int healAmount, int manaCost) : base(name, 1, false, manaCost)
    {
        HealAmount = healAmount;
    }

    protected override IEnumerator ApplyLogic(BaseEntity source, List<BaseEntity> targets)
    {
        targets.First().Heal(HealAmount);
        yield break;
    }
}