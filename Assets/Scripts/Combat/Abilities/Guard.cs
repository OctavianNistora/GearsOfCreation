using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.Combat.Abilities
{
    [CreateAssetMenu(fileName = "Guard", menuName = "Combat/Abilities/Guard")]
    public class Guard : BaseAction
    {
        public override int TargetCount => 0;
        public override bool TargetEnemy => false;
        
        [SerializeField]
        private int duration;
        [SerializeField]
        private float damageReductionPercent;

        protected override IEnumerator ApplyLogic(BaseEntity source, List<BaseEntity> targets)
        {
            var guardModifier = new DamageReceivedModifier(source, duration, (100f - damageReductionPercent) / 100f);
            
            source.AddModifier(guardModifier);
            CombatManager.Instance.OnCombatRoundEnd += guardModifier.RoundEnded;
            yield break;
        }
    }
}