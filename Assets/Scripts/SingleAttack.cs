using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Single-target Attack", menuName = "Combat/Abilities/Single-target Attack")]
public class SingleAttack : DamagingCharacterAbility
{
    public override int TargetCount => 1;
    public override bool TargetEnemy => true;

    protected override IEnumerator ApplyLogic(BaseEntity source, List<BaseEntity> targets)
    {
        if (targets.Count == 0)
        {
            yield break;
        }
        
        var target = targets.First();
        
        yield return target.CombatCharacterAnimationHandler.PlayVFXAnimation(VFXNumber);
        yield return target.Damage(Damage);
    }
}