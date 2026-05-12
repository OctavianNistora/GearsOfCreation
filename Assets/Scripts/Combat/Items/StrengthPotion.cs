using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace.Combat.Items
{
    [CreateAssetMenu(fileName = "Strength Potion", menuName = "Combat/Items/Strength Potion")]
    public class StrengthPotion : BaseCombatItem
    {
        public override int TargetCount => 1;
        public override bool TargetEnemy => false;

        [field: SerializeField]
        public int Duration { get; private set; }
        [field: SerializeField]
        public float DamageMultiplier { get; private set; }
        
        protected override IEnumerator ApplyLogic(BaseEntity source, List<BaseEntity> targets)
        {
            var target = targets.First();
            var strengthModifier = new DamageDealtModifier(target, Duration, DamageMultiplier);
            
            target.AddModifier(strengthModifier);
            CombatManager.Instance.OnCombatRoundEnd += strengthModifier.RoundEnded;
            yield break;
        }
    }
}