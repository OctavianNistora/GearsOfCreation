using UnityEngine;

[CreateAssetMenu(fileName = "MovementStrategy", menuName = "Scriptable Objects/MovementStrategy")]
public abstract class MovementStrategy : ScriptableObject
{
    public abstract void Move(
        Rigidbody2DPhysicsControl _rigidbodyControl,
        float input
    );
}
