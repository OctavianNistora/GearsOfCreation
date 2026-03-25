using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class ColliderContactEmitter : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Events events;
    
    private bool _wasMakingContact;
    private readonly List<Collider2D> _colliders = new();
    
    private Collider2D _collider;

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        var isMakingContact = _collider.GetContacts(_colliders) > 0;
        if (isMakingContact != _wasMakingContact)
        {
            events.onMakingContactStateChange.Invoke(isMakingContact);
            _wasMakingContact = isMakingContact;
        }
    }
    
    [Serializable]
    private class Events
    {
        public UnityEvent<bool> onMakingContactStateChange;
    }
}
