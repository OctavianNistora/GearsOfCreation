using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace.Combat.Items
{
    [CreateAssetMenu(fileName = "Health Potion", menuName = "Combat/Items/Health Potion")]
    public class HealthPotion : BaseCombatItem
    {
        public override int TargetCount => 1;
        public override bool TargetEnemy =>false;
        
        [field: SerializeField]
        public int HealAmount { get; private set; }
        
        protected override IEnumerator ApplyLogic(BaseEntity source, List<BaseEntity> targets)
        {
            targets.First().Heal(HealAmount);
            yield break;
        }
    }
}