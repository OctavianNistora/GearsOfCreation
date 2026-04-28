using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DefaultNamespace.Combat.Items
{
    public class HealthPotion : BaseCombatItem
    {
        public int HealAmount { get; private set; }
        
        public HealthPotion(string name, int quantity, int healAmount, int vfxNumber) : base(name, 1, false, quantity, vfxNumber)
        {
            HealAmount = healAmount;
        }

        protected override IEnumerator ApplyLogic(BaseEntity source, List<BaseEntity> targets)
        {
            targets.First().Heal(HealAmount);
            yield break;
        }
    }
}