using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.Combat.Abilities
{
    [CreateAssetMenu(fileName = "Rest", menuName = "Combat/Abilities/Rest")]
    public class Rest : BaseAction
    {
        public override int TargetCount => 0;
        public override bool TargetEnemy => false;
        
        [SerializeField]
        private int manaRestored = 20;
        
        protected override IEnumerator ApplyLogic(BaseEntity source, List<BaseEntity> targets)
        {
            if (source is PlayerEntity player)
            {
                player.RestoreMana(manaRestored);
            }
            yield break;
        }
    }
}