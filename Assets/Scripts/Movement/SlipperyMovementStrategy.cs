using UnityEngine;

[CreateAssetMenu(menuName = "Movement Strategies/Slippery")]
public class SlipperyMovementStrategy : MovementStrategy
{
    public float acceleration = 3f;

    public override void Move(
        Rigidbody2DPhysicsControl _rigidbodyControl,
        float input,
        float speed)
    {
        float target = input * speed;

        float velocity = Mathf.Lerp(
            _rigidbodyControl.GetHorizontalVelocity(),
            target,
            acceleration * Time.fixedDeltaTime
        );

        _rigidbodyControl.SetHorizontalVelocity(velocity);

    }
}
