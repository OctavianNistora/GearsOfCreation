using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseAction : ScriptableObject
{
    [field: SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    public string Description { get; private set; } = "Placeholder description.";
    public abstract int TargetCount {get; }
    public abstract bool TargetEnemy  { get; }
    [field: SerializeField]
    public int VFXNumber { get; private set; }

    private float _minimumWaitSeconds = 1f;

    public virtual IEnumerator Apply(BaseEntity source, List<BaseEntity> targets)
    {
        var minimumEndTime = Time.time  + _minimumWaitSeconds;
        
        var toastString = TargetCount > 0 && targets.Count > 0 ?
            $"{source.Name} used {Name} on {string.Join(", ", targets.Select(t => t.Name))}!" : 
            $"{source.Name} used {Name}!";
        ToastsHandler.Instance.CreateToastMessage(toastString);
        
        yield return ApplyLogic(source, targets);
        
        if (Time.time < minimumEndTime)
        {
            yield return new WaitForSeconds(minimumEndTime - Time.time);
        }
    }
    
    protected abstract IEnumerator ApplyLogic(BaseEntity source, List<BaseEntity> targets);

    public virtual BaseAction CreateInstance()
    {
        return Instantiate(this);
    }
}