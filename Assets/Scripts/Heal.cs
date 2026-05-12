using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal", menuName = "Combat/Abilities/Heal")]
public class Heal : HealingCharacterAbility
{
    public override int TargetCount => 1;
    public override bool TargetEnemy => false;

    protected override IEnumerator ApplyLogic(BaseEntity source, List<BaseEntity> targets)
    {
        var target = targets.First();
        
        yield return target.CombatCharacterAnimationHandler.PlayVFXAnimation(VFXNumber);
        target.Heal(HealAmount);
    }
}