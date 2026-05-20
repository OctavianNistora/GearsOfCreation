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
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            CombatManager.Instance.OnBattleEnded += HandleBattleEnd;
        }

        private void OnDestroy()
        {
            CombatManager.Instance.OnBattleEnded -= HandleBattleEnd;
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
            {
                return;
            }
            
            _encounterDefeated.Add(_currentEncounterId.Value);
        }
    }
}