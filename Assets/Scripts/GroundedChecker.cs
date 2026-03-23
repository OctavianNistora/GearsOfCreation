using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class GroundedChecker : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Events events;
    
    private bool _wasGrounded = true;
    
    private Collider2D _collider;
    private List<Collider2D> _ = new();

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        var isGrounded = _collider.GetContacts(_) <= 0;
        if (isGrounded != _wasGrounded)
        {
            events.onGroundedStateChange.Invoke(isGrounded);
            _wasGrounded = isGrounded;
        }
    }
    
    [Serializable]
    private class Events
    {
        public UnityEvent<bool> onGroundedStateChange;
    }
}
