using UnityEngine;

/// <summary>
/// MEMENTO
/// Stores an immutable snapshot of the player's state at a specific moment in time.
/// Used for save/load systems, rewind mechanics, undo systems, etc.
/// </summary>
public sealed class PlayerMemento
{
    // -------------------------
    // Transform Data
    // -------------------------
    public Vector3 Position { get; }
    public Quaternion Rotation { get; }
    public Vector3 Velocity { get; }
    public Vector3 AngularVelocity { get; }

    // -------------------------
    // Player Stats
    // -------------------------
    public float Health { get; }
    public float Stamina { get; }
    public int Score { get; }

    // -------------------------
    // Metadata
    // -------------------------
    public float TimeStamp { get; }

    /// <summary>
    /// Creates a new immutable snapshot of the player state.
    /// </summary>
    public PlayerMemento(
        Vector3 position,
        Quaternion rotation,
        Vector3 velocity,
        Vector3 angularVelocity,
        float health,
        float stamina,
        int score)
    {
        Position = position;
        Rotation = rotation;
        Velocity = velocity;
        AngularVelocity = angularVelocity;

        Health = health;
        Stamina = stamina;
        Score = score;

        TimeStamp = Time.time;
    }
}