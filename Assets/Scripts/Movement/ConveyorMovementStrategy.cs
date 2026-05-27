using UnityEngine;

public enum ConveyorDirection
{
    Left = -1,
    Right = 1
}

[CreateAssetMenu(menuName = "Movement Strategies/Conveyor Belt")]
public class ConveyorMovementStrategy : MovementStrategy
{
    [SerializeField] private float conveyorSpeed = 3f;

    [SerializeField]
    private ConveyorDirection direction;

    public override void Move(
        Rigidbody2DPhysicsControl _rigidbodyControl,
        float horizontalInput,
        float moveSpeed)
    {
        float playerVelocity =
            horizontalInput * moveSpeed;

        float conveyorVelocity =
            conveyorSpeed * (int)direction;

        _rigidbodyControl.SetHorizontalVelocity(playerVelocity + conveyorVelocity);
    }
}
