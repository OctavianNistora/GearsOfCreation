using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace
{
    public class ButtonConditionRepeater : AbstractConditionEmitter
    {
        [SerializeField] private bool invertCondition;

        public void CheckCondition(InputAction.CallbackContext context)
        {
            bool repeatedValue;
            if (context.started)
            {
                repeatedValue = !invertCondition;
            }
            else if (context.canceled)
            {
                repeatedValue = invertCondition;
            }
            else
            {
                return;
            }
            
            OnConditionChange.Invoke(this, repeatedValue);
        }
    }
}