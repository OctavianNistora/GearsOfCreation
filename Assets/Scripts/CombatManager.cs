using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }
    
    public List<EnemyEntity> enemies = new();
    public event Action OnRoundStart;
    public event Action OnRoundEnd;
    public event Action onBattleWon;
    
    private PlayerEntity selectedSource;
    private BaseAbility selectedAbility;
    private List<BaseEntity> selectedTargets;
    private Dictionary<PlayerEntity, SourceActionData> sourceActions = new();
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public List<PlayerEntity> GetActionedAllies()
    {
        return sourceActions.Keys.ToList();
    }

    public void SelectSource(PlayerEntity entity)
    {
        selectedSource = entity;
        
        Debug.Log("Selected source: " + entity.Name);
    }
    
    public PlayerEntity GetSelectedSource()
    {
        return selectedSource;
    }

    public bool SelectAbility(BaseAbility ability)
    {
        selectedAbility = ability;
        selectedTargets = new List<BaseEntity>();
        
        var needsContinue = selectedAbility.TargetCount > 0;
        
        if (!needsContinue)
        {
            sourceActions[selectedSource] = new SourceActionData
            {
                Ability = selectedAbility,
                Targets = selectedAbility.TargetEnemy ? 
                    enemies.Select(entity => entity as BaseEntity).ToList() :
                    PartyManager.Instance.members.Select(entity => entity as BaseEntity).ToList()
            };
            
            if (sourceActions.Count >= PartyManager.Instance.members.Count)
            {
                StartCoroutine(StartRound());
            }
        }

        Debug.Log("Selected ability: " + ability.Name);
        return needsContinue;
    }
    
    public BaseAbility GetSelectedAbility()
    {
        return selectedAbility;
    }
    
    public bool SelectTarget(BaseEntity entity)
    {
        selectedTargets.Add(entity);
        
        var needsContinue = selectedTargets.Count < selectedAbility.TargetCount && 
                            !(
                                selectedAbility.TargetEnemy && selectedTargets.Count >= enemies.Count || 
                                !selectedAbility.TargetEnemy && selectedTargets.Count >= PartyManager.Instance.members.Count
                                );

        if (!needsContinue)
        {
            sourceActions[selectedSource] = new SourceActionData
            {
                Ability = selectedAbility,
                Targets = selectedTargets
            };
            
            if (sourceActions.Count >= PartyManager.Instance.members.Count)
            {
                StartCoroutine(StartRound());
            }
        }
        
        Debug.Log("Selected target: " + entity.Name);
        return needsContinue;
    }

    public List<PlayerEntity> GetReadyAllies()
    {
        return sourceActions.Keys.ToList();
    }

    public IEnumerator StartRound()
    {
        OnRoundStart?.Invoke();
        
        foreach (var sourceAction in sourceActions)
        {
            yield return sourceAction.Value.Ability.Apply(sourceAction.Key, sourceAction.Value.Targets);
        }
        
        yield return new WaitForSeconds(1.5f);

        foreach (var enemy in enemies)
        {
            if (enemy.CurrentHp <= 0)
            {
                continue;
            }
            
            var ability = enemy.Abilities[Random.Range(0, enemy.Abilities.Count)];

            if (ability.TargetCount <= 0)
            {
                yield return ability.Apply(enemy, PartyManager.Instance.members.Where(member => member.CurrentHp > 0).Select(entity => entity as BaseEntity).ToList());
                continue;
            }

            List<BaseEntity> targets;
            if (ability.TargetEnemy)
            {
                targets = PartyManager.Instance.members.Where(member => member.CurrentHp > 0).OrderBy(qu => Guid.NewGuid()).Take(ability.TargetCount).Select(entity => entity as BaseEntity).ToList();
                yield return ability.Apply(enemy, targets);
            }
            else
            {
                targets = enemies.Where(e => e.CurrentHp > 0).OrderBy(qu => Guid.NewGuid()).Take(ability.TargetCount).Select(entity => entity as BaseEntity).ToList();
                yield return ability.Apply(enemy, targets);
            }
        }
        
        sourceActions.Clear();
        
        if (enemies.All(enemy => enemy.CurrentHp <= 0))
        {
            Debug.Log("Victory!");
            onBattleWon?.Invoke();
            yield break;
        }
        OnRoundEnd?.Invoke();
    }
}

public struct SourceActionData
{
    public BaseAbility Ability { get; set; }
    public List<BaseEntity> Targets { get; set; }
}