using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.Combat.Abilities
{
    public class Rest : BaseAction
    {
        private readonly int _manaRestored = 20;
        
        public Rest() : base("Rest", 0, false, 0)
        {
        }

        protected override IEnumerator ApplyLogic(BaseEntity source, List<BaseEntity> targets)
        {
            if (source is PlayerEntity player)
            {
                player.RestoreMana(_manaRestored);
            }
            yield break;
        }
    }
}