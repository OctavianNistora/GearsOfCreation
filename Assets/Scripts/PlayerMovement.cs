using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2DPhysicsControl _rigidbodyControl;
    [SerializeField] private Animator _animator;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private PlayerMovementStats movementStats;
    
    [Header("Ground Check")]
    [SerializeField] private GameObject groundCheckBoxCenter;
    [SerializeField] private Vector2 groundCheckBoxSize;

    [Header("Constants")]
    //[SerializeField] private float horizontalMovementSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private int airJumps;

    public bool _canGroundJump;
    public bool _canEndJumpEarly;
    public int _remainingAirJumps;

    private float currentAcceleration;
    private float currentDeceleration;
    private bool _isGrounded = true;
    private bool jumpPressed = false;
    private bool jumpReleased = false;

    //jump buffer vars
    private float _jumpBufferTimeCounter;

    //coyote time vars
    private float _coyoteTimeCounter;

    [SerializeField] private MovementStrategy defaultStrategy;
    private MovementStrategy currentStrategy;

    public void Start()
    {
        currentAcceleration = movementStats.groundAcceleration;
        currentDeceleration = movementStats.groundDeceleration;

        currentStrategy = defaultStrategy;
        ResetInput();
    }

    public void FixedUpdate()
    {
        DetectTileUnderPlayer();
        JumpChecks();
    }

    async void ResetInput()
    {
        gameObject.GetComponent<PlayerInput>().enabled = false;
        await Task.Delay(1000);
        gameObject.GetComponent<PlayerInput>().enabled = true;
    }

    public void CheckCanStillEndJumpEarly(float verticalVelocity)
    {
        if (!_canEndJumpEarly) return;

        _canEndJumpEarly = verticalVelocity > 0;
    }

    public void MoveHorizontal(InputAction.CallbackContext context)
    {
        var horizontal = context.ReadValue<float>();

        float targetVelocity = horizontal * movementStats.maxWalkSpeed;
        float newVelocity = 0f;

        if (Mathf.Approximately(horizontal, 0))
        {
            _animator.SetBool("walk", false);
            newVelocity = Mathf.Lerp(newVelocity, 0, currentDeceleration * Time.fixedDeltaTime);
        }
        else
        {
            _animator.SetBool("walk", true);
            newVelocity = Mathf.Lerp(newVelocity, targetVelocity, currentAcceleration * Time.fixedDeltaTime);
        }

        //_rigidbodyControl.SetHorizontalVelocity(horizontal * horizontalMovementSpeed);


        currentStrategy.Move(_rigidbodyControl, newVelocity);
    }

    public void PlayWalkSound()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.walk);
    }

    public void JumpChecks()
    {
        if(_isGrounded)
        {
            _coyoteTimeCounter = movementStats.jumpCoyoteTime;
        }
        else
        {
            if (_coyoteTimeCounter > 0f)
                _coyoteTimeCounter -= Time.fixedDeltaTime;
        }

        if (_jumpBufferTimeCounter > 0f)
            _jumpBufferTimeCounter -= Time.fixedDeltaTime;

        if (_coyoteTimeCounter > 0f && _jumpBufferTimeCounter > 0f)
        {
            Jump();
            _jumpBufferTimeCounter = 0f;
        }
    }

    public void ControlJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Jump();
            _jumpBufferTimeCounter = movementStats.jumpBufferTime;
            jumpPressed = true;
            jumpReleased = false;
        }
        else if (context.canceled)
        {
            StopJumpEarly();
            _coyoteTimeCounter = 0f;
            jumpPressed = false;
            jumpReleased = true;
        }
    }

    public void OnGroundedStateChange(bool isGrounded)
    {
        _isGrounded = isGrounded;

        _animator.SetBool("mid_air", !isGrounded);
        if (isGrounded)
        {
            currentAcceleration = movementStats.groundAcceleration;
            currentDeceleration = movementStats.groundDeceleration;

            _canGroundJump = true;
            _remainingAirJumps = airJumps;
        }
        else
        {
            currentAcceleration = movementStats.airAcceleration;
            currentDeceleration = movementStats.airDeceleration;
        }
    }

    private void Jump()
    {
        if (!_isGrounded && _coyoteTimeCounter <= 0)
            return;

        if (_canGroundJump)
        {
            _canGroundJump = false;
        }
        else
        {
            if (_remainingAirJumps <= 0) return;
            _remainingAirJumps--;
        }

        _rigidbodyControl.NegateNegativeVerticalVelocity();
        _rigidbodyControl.AddUpwardsImpulse(jumpForce);

        AudioManager.Instance.PlaySFX(AudioManager.Instance.jump);

        _canEndJumpEarly = true;
    }

    private void StopJumpEarly()
    {
        if (!_canEndJumpEarly) return;

        _canEndJumpEarly = false;
        _rigidbodyControl.ReduceUpwardsVelocity();
    }

    private void DetectTileUnderPlayer()
    {
        Vector3Int cellPosition =
            groundTilemap.WorldToCell(
                transform.position + Vector3.down * 0.1f
            );

        TileBase tile = groundTilemap.GetTile(cellPosition);

        if (tile is MovementTile movementTile && movementTile.strategy != null)
        {
            if (currentStrategy == movementTile.strategy) 
                return;
            currentStrategy = movementTile.strategy;
            currentStrategy.Move(_rigidbodyControl, 0);
        }
        else
        {
            if (tile is AnimatedMovementTile animatedMovementTile && animatedMovementTile.strategy != null)
            {
                if (currentStrategy == animatedMovementTile.strategy) 
                    return;
                currentStrategy = animatedMovementTile.strategy;
                currentStrategy.Move(_rigidbodyControl, 0);
            }
            else
                currentStrategy = defaultStrategy;
        }
    }
}