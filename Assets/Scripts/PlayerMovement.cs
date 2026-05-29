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
    
    [Header("Ground Check")]
    [SerializeField] private GameObject groundCheckBoxCenter;
    [SerializeField] private Vector2 groundCheckBoxSize;

    [Header("Constants")]
    [SerializeField] private float horizontalMovementSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private int airJumps;

    public bool _canGroundJump;
    public bool _canEndJumpEarly;
    public int _remainingAirJumps;

    [SerializeField] private MovementStrategy defaultStrategy;
    private MovementStrategy currentStrategy;

    public void Start()
    {
        currentStrategy = defaultStrategy;
        ResetInput();
    }

    public void Update()
    {
        DetectTileUnderPlayer();
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

        if (Mathf.Approximately(horizontal, 0))
        {
            _animator.SetBool("walk", false);
        }
        else
        {
            _animator.SetBool("walk", true);
        }

        //_rigidbodyControl.SetHorizontalVelocity(horizontal * horizontalMovementSpeed);
        currentStrategy.Move(_rigidbodyControl, horizontal, horizontalMovementSpeed);
    }

    public void PlayWalkSound()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.walk);
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
        _animator.SetBool("mid_air", !isGrounded);
        if (isGrounded)
        {
            _canGroundJump = true;
            _remainingAirJumps = airJumps;
        }
    }

    private void Jump()
    {
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
            currentStrategy.Move(_rigidbodyControl, 0, horizontalMovementSpeed);
        }
        else
        {
            if (tile is AnimatedMovementTile animatedMovementTile && animatedMovementTile.strategy != null)
            {
                if (currentStrategy == animatedMovementTile.strategy) 
                    return;
                currentStrategy = animatedMovementTile.strategy;
                currentStrategy.Move(_rigidbodyControl, 0, horizontalMovementSpeed);
            }
            else
                currentStrategy = defaultStrategy;
        }
    }
}