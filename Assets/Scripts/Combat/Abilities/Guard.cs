using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.Combat.Abilities
{
    public class Guard : BaseAction
    {
        public Guard() : base("Guard", 0, false)
        {
        }

        protected override IEnumerator ApplyLogic(BaseEntity source, List<BaseEntity> targets)
        {
            var guardModifier = new DamageReceivedModifier(source, 1, 0.5f);
            
            source.AddModifier(guardModifier);
            CombatManager.Instance.OnCombatRoundEnd += guardModifier.RoundEnded;
            yield break;
        }
    }
}