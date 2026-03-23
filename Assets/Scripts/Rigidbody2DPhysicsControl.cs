using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class Rigidbody2DPhysicsControl : MonoBehaviour
{
    [Header("Constants")]
    [SerializeField] private float maxFallSpeed;
    
    [Header("")]
    [SerializeField] private Events events;
    
    private float _horizontalVelocity;
    private float _previousVerticalVelocity;
    
    private Rigidbody2D _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _previousVerticalVelocity = _rigidbody.linearVelocity.y;
    }

    private void FixedUpdate()
    {
        _rigidbody.linearVelocity = new Vector2(_horizontalVelocity, _rigidbody.linearVelocity.y);
        
        if (_rigidbody.linearVelocity.y < 0)
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x,
                Mathf.Max(_rigidbody.linearVelocity.y, -maxFallSpeed));

        if (!Mathf.Approximately(_rigidbody.linearVelocity.y, _previousVerticalVelocity))
        {
            events.onVerticalVelocityChange.Invoke(_rigidbody.linearVelocity.y);
            _previousVerticalVelocity = _rigidbody.linearVelocity.y;
        }
    }

    public bool IsGainingHeight()
    {
        return _rigidbody.linearVelocity.y > 0;
    }

    public void SetHorizontalVelocity(float horizontalVelocity)
    {
        _horizontalVelocity = horizontalVelocity;
    }

    public void AddUpwardsImpulse(float verticalImpulse)
    {
        if (_rigidbody.linearVelocity.y < 0)
        {
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0);
        }
        
        _rigidbody.AddForce(Vector2.up * verticalImpulse, ForceMode2D.Impulse);
    }

    public void ReduceUpwardsVelocity()
    {
        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x,
            Mathf.Min(_rigidbody.linearVelocity.y / 2, _rigidbody.linearVelocity.y));
    }
    
    [Serializable]
    private class Events
    {
        public UnityEvent<float> onVerticalVelocityChange;
    }
}