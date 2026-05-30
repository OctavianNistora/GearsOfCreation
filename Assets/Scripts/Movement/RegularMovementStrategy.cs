using UnityEngine;

[CreateAssetMenu(menuName = "Movement Strategies/Regular")]
public class RegularMovementStrategy : MovementStrategy
{
    public override void Move(
        Rigidbody2DPhysicsControl _rigidbodyControl,
        float input)
    {
        _rigidbodyControl.SetHorizontalVelocity(input);
    }
}
