using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDirection : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprite;
    public void ChangeDirection(InputAction.CallbackContext context)
    {
        var direction = context.ReadValue<float>();
        if (!Mathf.Approximately(direction, 0) && (context.started || context.canceled))
        {
            var degrees = direction > 0 ? 0 : 180;
            
            _sprite.flipX = direction > 0;
            
            transform.localRotation = Quaternion.AngleAxis(degrees, Vector3.up);
        }
    }
}
