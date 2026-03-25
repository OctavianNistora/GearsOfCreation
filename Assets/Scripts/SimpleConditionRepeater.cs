using UnityEngine;

namespace DefaultNamespace
{
    public class SimpleConditionRepeater : AbstractConditionEmitter
    {
        [SerializeField] private bool invertCondition;

        public void CheckCondition(bool value)
        {
            var repeatedValue = invertCondition ? !value : value;
            OnConditionChange.Invoke(this, repeatedValue);
        }
    }
}