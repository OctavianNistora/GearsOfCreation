using System;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public abstract class AbstractConditionEmitter : MonoBehaviour
    { 
        public Action<AbstractConditionEmitter, bool> OnConditionChange = delegate { };
    }
}