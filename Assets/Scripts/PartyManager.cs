using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance { get; private set; }
    
    public List<PlayerEntity> members = new();
    
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
    public List<BaseAbility> Abilities { get; private set; }

    public event Action OnDamageTaken;
    public event Action OnHeal;
    public event Action OnDeath;

    protected BaseEntity(string name, int maxHp, int currentHp, List<BaseAbility> abilities)
    {
        Name = name;
        MaxHp = maxHp;
        CurrentHp = currentHp;
        Abilities = abilities;
    }
    
    public void Damage(int damage)
    {
        if (CurrentHp <= 0) return;
        
        CurrentHp -= damage;
        ToastsHandler.Instance.CreateToastMessage($"{Name} took {damage} damage!");
        OnDamageTaken?.Invoke();
        
        if (CurrentHp > 0) return;
        CurrentHp = 0;
        ToastsHandler.Instance.CreateToastMessage($"{Name} has died!");
        OnDeath?.Invoke();
    }
    
    public void  Heal(int heal)
    {
        if (CurrentHp <= 0) return;
        
        CurrentHp += heal;
        ToastsHandler.Instance.CreateToastMessage($"{Name} was healed for {heal} hp!");
        OnHeal?.Invoke();
        
        if (CurrentHp > MaxHp) CurrentHp = MaxHp;
        
    }
}

public class EnemyEntity : BaseEntity
{
    public EnemyEntity(string name, int maxHp, int currentHp, List<BaseAbility> abilities) : base(name, maxHp, currentHp, abilities)
    {
    }
}

public class PlayerEntity : BaseEntity
{
    public PlayerEntity(string name, int maxHp, int currentHp, int maxMana, int currentMana, List<BaseAbility> abilities) : base(name, maxHp, currentHp, abilities)
    {
        MaxMana = maxMana;
        CurrentMana = currentMana;
    }

    public int MaxMana { get; private set; }
    public int CurrentMana { get; private set; }
    
    
}

public abstract class BaseAbility
{
    public string Name { get; private set; }
    public int TargetCount {get; private set;}
    public bool TargetEnemy  { get; private set; }
    public abstract IEnumerator Apply(BaseEntity source, List<BaseEntity> targets);
    
    protected BaseAbility(string name, int targetCount, bool targetEnemy)
    {
        Name = name;
        TargetCount = targetCount;
        TargetEnemy = targetEnemy;
    }
}

public class SingleAttack : BaseAbility
{
    public int Damage { get; private set; }
    
    public SingleAttack(string name, int damage) : base(name, 1, true)
    {
        Damage = damage;
    }

    public override IEnumerator Apply(BaseEntity source, List<BaseEntity> targets)
    {
        ToastsHandler.Instance.CreateToastMessage($"{source.Name} used {Name}!");
        yield return new WaitForSeconds(1.5f);
        
        targets.First().Damage(Damage);
    }
}

public class AoeAttack : BaseAbility
{
    public int Damage { get; private set; }
    
    public AoeAttack(string name, int damage) : base(name, 0, true)
    {
        Damage = damage;
    }

    public override IEnumerator Apply(BaseEntity source, List<BaseEntity> targets)
    {
        ToastsHandler.Instance.CreateToastMessage($"{source.Name} used {Name}!");
        yield return new WaitForSeconds(1.5f);
        
        targets.ForEach(target => target.Damage(Damage));
    }
}

public class Heal : BaseAbility
{
    public int HealAmount { get; private set; }
    
    public Heal(string name, int healAmount) : base(name, 1, false)
    {
        HealAmount = healAmount;
    }

    public override IEnumerator Apply(BaseEntity source, List<BaseEntity> targets)
    {
        ToastsHandler.Instance.CreateToastMessage($"{source.Name} used {Name}!");
        yield return new WaitForSeconds(1.5f);
        
        targets.First().Heal(HealAmount);
    }
}