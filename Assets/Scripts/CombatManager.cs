using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    public EncounterType currentEncounterType;
    public List<EnemyEntity> enemies = new();

    public event Action OnPlayerChoiceStart;
    public event Action OnPlayerChoiceEnd;
    public event Action OnCombatRoundStart;
    public event Action OnCombatRoundEnd;
    public event Action<BattleEndStateEnum> OnBattleEnded;

    private PlayerEntity _selectedSource;
    private BaseAction _selectedAction;
    private List<BaseEntity> _selectedTargets;
    private Dictionary<PlayerEntity, SourceActionData> _sourceActions = new();
    private bool _isEscaping;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void InitializeCombat(List<EnemyEntity> newEnemies)
    {
        enemies = newEnemies;

        OnPlayerChoiceStart = null;
        OnPlayerChoiceEnd = null;
        OnCombatRoundStart = null;
        OnCombatRoundEnd = null;
        OnBattleEnded = null;

        _sourceActions.Clear();
        _selectedSource = null;
        _selectedAction = null;
        _selectedTargets = null;

        _isEscaping = false;
    }

    public List<PlayerEntity> GetActionedAllies()
    {
        return _sourceActions.Keys.ToList();
    }

    public void SelectSource(PlayerEntity entity)
    {
        _selectedSource = entity;
    }

    public PlayerEntity GetSelectedSource()
    {
        return _selectedSource;
    }

    public bool SelectAbility(BaseAction action)
    {
        _selectedAction = action;
        _selectedTargets = new List<BaseEntity>();

        var needsContinue = _selectedAction.TargetCount > 0;

        if (!needsContinue)
        {
            _sourceActions[_selectedSource] = new SourceActionData
            {
                action = _selectedAction,
                Targets = _selectedAction.TargetEnemy
                    ? enemies.Select(e => (BaseEntity)e).ToList()
                    : PartyManager.Instance.Members.Select(e => (BaseEntity)e).ToList()
            };

            if (_sourceActions.Count >= PartyManager.Instance.Members.Count)
            {
                StartCoroutine(StartRound());
            }
        }

        return needsContinue;
    }

    public BaseAction GetSelectedAbility()
    {
        return _selectedAction;
    }

    public bool SelectTarget(BaseEntity entity)
    {
        _selectedTargets.Add(entity);

        var needsContinue =
            _selectedTargets.Count < _selectedAction.TargetCount &&
            !(
                _selectedAction.TargetEnemy && _selectedTargets.Count >= enemies.Count ||
                !_selectedAction.TargetEnemy && _selectedTargets.Count >= PartyManager.Instance.Members.Count
            );

        if (!needsContinue)
        {
            _sourceActions[_selectedSource] = new SourceActionData
            {
                action = _selectedAction,
                Targets = _selectedTargets
            };

            if (_sourceActions.Count >= PartyManager.Instance.Members.Count)
            {
                StartCoroutine(StartRound());
            }
        }

        return needsContinue;
    }

    public List<PlayerEntity> GetReadyAllies()
    {
        return _sourceActions.Keys.ToList();
    }

    public void AttemptEscape()
    {
        CheckEscapeRng(1f);
        _sourceActions.Clear();
        StartCoroutine(StartRound());
    }

    public int GetItemUsedQuantity(BaseCombatItem item)
    {
        int quantityUsed = 0;

        foreach (var action in _sourceActions.Values)
        {
            if (action.action == item)
                quantityUsed++;
        }

        return quantityUsed;
    }

    public void SetEncounter(CombatEncounter encounter)
    {
        enemies.Clear();

        encounter.enemies.ForEach(enemy =>
        {
            var instance = (EnemyEntity)enemy.CreateInstance();
            enemies.Add(instance);
        });
    }

    private void CheckEscapeRng(float chance)
    {
        var roll = Random.Range(0f, 1f);
        if (roll <= chance)
            _isEscaping = true;
    }

    private IEnumerator StartRound()
    {
        OnPlayerChoiceEnd?.Invoke();
        OnCombatRoundStart?.Invoke();

        foreach (var sourceAction in _sourceActions)
        {
            yield return sourceAction.Value.action.Apply(
                sourceAction.Key,
                sourceAction.Value.Targets
            );
        }

        yield return new WaitForSeconds(1.5f);

        foreach (var enemy in enemies)
        {
            if (enemy.CurrentHp <= 0)
                continue;

            var ability = enemy.Abilities[Random.Range(0, enemy.Abilities.Count)];

            List<BaseEntity> targets;

            if (ability.TargetCount <= 0)
            {
                yield return ability.Apply(
                    enemy,
                    PartyManager.Instance.Members
                        .Where(m => m.CurrentHp > 0)
                        .Cast<BaseEntity>()
                        .ToList()
                );

                continue;
            }

            if (ability.TargetEnemy)
            {
                targets = PartyManager.Instance.Members
                    .Where(m => m.CurrentHp > 0)
                    .OrderBy(_ => Guid.NewGuid())
                    .Take(ability.TargetCount)
                    .Cast<BaseEntity>()
                    .ToList();
            }
            else
            {
                targets = enemies
                    .Where(e => e.CurrentHp > 0)
                    .OrderBy(_ => Guid.NewGuid())
                    .Take(ability.TargetCount)
                    .Cast<BaseEntity>()
                    .ToList();
            }

            yield return ability.Apply(enemy, targets);
        }

        _sourceActions.Clear();
        OnCombatRoundEnd?.Invoke();

        // =========================
        // VICTORY
        // =========================
        if (enemies.All(e => e.CurrentHp <= 0))
        {
            Debug.Log("Victory!");

            OnBattleEnded?.Invoke(BattleEndStateEnum.Victory);

            // 🔥 IMPORTANT: prevenim LoadGame automat în PlatformerScene
            CheckpointManager.SkipAutoLoadOnce = true;

            FadeToPlatformerScene();
            yield break;
        }

        // =========================
        // DEFEAT
        // =========================
        if (PartyManager.Instance.Members.All(m => m.CurrentHp <= 0))
        {
            Debug.Log("Defeat!");

            OnBattleEnded?.Invoke(BattleEndStateEnum.Defeat);

            FadeToPlatformerScene(); 
            // (dacă ai LoadGame pe defeat în alt script, rămâne acolo controlul)

            yield break;
        }

        // =========================
        // ESCAPE
        // =========================
        if (_isEscaping)
        {
            Debug.Log("Escaped!");

            OnBattleEnded?.Invoke(BattleEndStateEnum.Escape);

            FadeToPlatformerScene();
            yield break;
        }

        OnPlayerChoiceStart?.Invoke();
    }

    private async void FadeToPlatformerScene()
    {
        await FadeManager.Instance.FadeToBlack();
        await SceneManager.LoadSceneAsync("PlatformerScene");
        await FadeManager.Instance.FadeToTransparent();
    }
}

public struct SourceActionData
{
    public BaseAction action { get; set; }
    public List<BaseEntity> Targets { get; set; }
}

public enum BattleEndStateEnum
{
    Victory,
    Defeat,
    Escape
}