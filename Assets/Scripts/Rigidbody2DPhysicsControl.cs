using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class Rigidbody2DPhysicsControl : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovementStats movementStats;

    [Header("Constants")]
    [SerializeField] private float maxFallSpeed;
    
    [Header("")]
    [SerializeField] private Events events;
    
    private float _horizontalVelocity;
    private float _previousVerticalVelocity;
    private bool _physicsEnabled;
    private Vector2 _storedLinearVelocity;
    
    private Rigidbody2D _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _previousVerticalVelocity = _rigidbody.linearVelocity.y;
        _physicsEnabled = true;
    }

    private void FixedUpdate()
    {
        if (!_physicsEnabled)
        {
            return;
        }
        
        _rigidbody.linearVelocity = new Vector2(_horizontalVelocity, _rigidbody.linearVelocity.y);
        
        
        if (_rigidbody.linearVelocity.y < 0)
        {
            _rigidbody.gravityScale = movementStats.baseGravity * movementStats.fallSpeedMultiplier;

            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x,
            Mathf.Max(_rigidbody.linearVelocity.y, -movementStats.maxFallSpeed));
        }
        else
        {
            _rigidbody.gravityScale = movementStats.baseGravity;
        }

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

    public float GetVerticalVelocity()
    {
        return _rigidbody.linearVelocity.y;
    }

    public void SetVerticalVelocity(float verticalVelocity)
    {
        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, verticalVelocity);
    }

    public float GetHorizontalVelocity()
    {
        return _horizontalVelocity;
    }

    public void SetHorizontalVelocity(float horizontalVelocity)
    {
        _horizontalVelocity = horizontalVelocity;
    }

    public void NegateNegativeVerticalVelocity()
    {
        if (_rigidbody.linearVelocity.y < 0)
        {
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0);
        }
    }

    public void AddUpwardsImpulse(float verticalImpulse)
    {
        _rigidbody.AddForce(Vector2.up * verticalImpulse, ForceMode2D.Impulse);
    }

    public void ReduceUpwardsVelocity()
    {
        if (!_physicsEnabled)
        {
            return;
        }
        
        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x,
            Mathf.Min(_rigidbody.linearVelocity.y / 2, _rigidbody.linearVelocity.y));
    }

    public void PausePhysics()
    {
        _physicsEnabled = false;
        
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _storedLinearVelocity  = _rigidbody.linearVelocity;
        _rigidbody.linearVelocity = Vector2.zero;
    }

    public void RestartPhysics()
    {
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        
        _physicsEnabled = true;
    }
    
    [Serializable]
    private class Events
    {
        public UnityEvent<float> onVerticalVelocityChange;
    }
}