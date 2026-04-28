using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DefaultNamespace.Combat.Items
{
    public class StrengthPotion : BaseCombatItem
    {
        public int Duration { get; private set; }
        public float DamageMultiplier { get; private set; }
        
        public StrengthPotion(string name, int quantity, int duration, float damageMultiplier, int vfxNumber) : base(name, 1, false, quantity, vfxNumber)
        {
            Duration = duration;
            DamageMultiplier = damageMultiplier;
        }

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