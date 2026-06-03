using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class EncounterProgressManager : MonoBehaviour
    {
        public static EncounterProgressManager Instance;

        private Guid? _currentEncounterId;
        private readonly HashSet<Guid> _encounterDefeated = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            // 🔥 SAFE SUBSCRIBE (avoid double subscribe after scene reload)
            if (CombatManager.Instance != null)
            {
                CombatManager.Instance.OnBattleEnded += HandleBattleEnd;
            }
        }

        private void OnDisable()
        {
            if (CombatManager.Instance != null)
            {
                CombatManager.Instance.OnBattleEnded -= HandleBattleEnd;
            }
        }

        public void HandleBattleEnd(BattleEndStateEnum battleEndState)
        {
            if (battleEndState == BattleEndStateEnum.Victory)
            {
                MarkCurrentEncounterDefeated();
            }
        }

        public bool IsEncounterDefeated(Guid encounterId)
        {
            return _encounterDefeated.Contains(encounterId);
        }

        public void ChangeCurrentEncounter(Guid? encounterId)
        {
            _currentEncounterId = encounterId;
        }

        public void MarkCurrentEncounterDefeated()
        {
            if (_currentEncounterId == null)
                return;

            _encounterDefeated.Add(_currentEncounterId.Value);
        }

        public void ResetDefeatedEncounters(List<string> defeatedIDs)
        {
            _encounterDefeated.Clear();

            foreach (var id in defeatedIDs)
            {
                if (Guid.TryParse(id, out Guid guid))
                    _encounterDefeated.Add(guid);
            }
        }
    }
}