using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementStats", menuName = "Scriptable Objects/PlayerMovementStats")]
public class PlayerMovementStats : ScriptableObject
{
    [Header("Horizontal Movement")]
    [Range(1f, 100f)] public float maxWalkSpeed = 12.5f;
    [Range(0.25f, 50f)] public float groundAcceleration = 5f;
    [Range(0.25f, 50f)] public float groundDeceleration = 20f;
    [Range(0.25f, 50f)] public float airAcceleration = 5f;
    [Range(0.25f, 50f)] public float airDeceleration = 5f;

    [Header("Jump")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;
}
