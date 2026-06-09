using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AOE Attack", menuName = "Combat/Abilities/AOE Attack")]
public class AoeAttack : DamagingCharacterAbility
{
    public override int TargetCount => 0;
    public override bool TargetEnemy => true;

    protected override IEnumerator ApplyLogic(BaseEntity source, List<BaseEntity> targets)
    {
        if (targets.Count == 0)
        {
            yield break;
        }

        var animationEnumeratorList = new List<IEnumerator>();
        targets.ForEach(target => animationEnumeratorList.Add(Wrapper(source, target)));

        foreach (var animationEnumerator in animationEnumeratorList)
        {
            yield return animationEnumerator;
        }
    }

    private IEnumerator Wrapper(BaseEntity source, BaseEntity target)
    {
        yield return target.CombatCharacterAnimationHandler.PlayVFXAnimation(VFXNumber);
        yield return target.Damage(Damage);
    }
}