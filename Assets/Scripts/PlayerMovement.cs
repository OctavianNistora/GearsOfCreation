using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2DPhysicsControl _rigidbodyControl;

    [Header("Ground Check")]
    [SerializeField] private GameObject groundCheckBoxCenter;
    [SerializeField] private Vector2 groundCheckBoxSize;

    [Header("Constants")]
    [SerializeField] private float horizontalMovementSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private int airJumps;

    private bool _canEndJumpEarly;
    private int _remainingJumps;

    private void Start()
    {
        _remainingJumps = airJumps;
    }

    public void CheckCanStillEndJumpEarly(float verticalVelocity)
    {
        if (!_canEndJumpEarly) return;

        _canEndJumpEarly = verticalVelocity > 0;
    }

    public void MoveHorizontal(InputAction.CallbackContext context)
    {
        var horizontal = context.ReadValue<float>();

        _rigidbodyControl.SetHorizontalVelocity(horizontal * horizontalMovementSpeed);
    }

    public void ControlJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Jump();
        }
        else if (context.canceled)
        {
            StopJumpEarly();
        }
    }

    public void OnGroundedStateChange(bool isGrounded)
    {
        if (isGrounded)
        {
            _remainingJumps = airJumps;
        }
    }

    private void Jump()
    {
        var isGrounded = Physics2D.OverlapBox(groundCheckBoxCenter.transform.position, groundCheckBoxSize, 0, LayerMask.GetMask("Terrain"));
        if (!isGrounded)
        {
            if (_remainingJumps <= 0) return;
            _remainingJumps--;
        }

        _rigidbodyControl.AddUpwardsImpulse(jumpForce);
        _canEndJumpEarly = true;
    }

    private void StopJumpEarly()
    {
        if (!_canEndJumpEarly) return;

        _canEndJumpEarly = false;
        _rigidbodyControl.ReduceUpwardsVelocity();
    }
}