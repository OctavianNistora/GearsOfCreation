using System;

/// <summary>
/// Plain serializable save container.
/// Only primitives/string values allowed.
/// </summary>
[Serializable]
public class SaveData
{
    // ─────────────────────────────────────────────────────────────────────────
    // PLAYER TRANSFORM
    // ─────────────────────────────────────────────────────────────────────────

    public float PlayerX;
    public float PlayerY;
    public float RotationZ;

    // ─────────────────────────────────────────────────────────────────────────
    // PLAYER STATS
    // ─────────────────────────────────────────────────────────────────────────

    public float Health;
    public float Stamina;
    public int Score;

    // ─────────────────────────────────────────────────────────────────────────
    // CHECKPOINT
    // ─────────────────────────────────────────────────────────────────────────

    public string LastCheckpointName;

    // ─────────────────────────────────────────────────────────────────────────
    // META
    // ─────────────────────────────────────────────────────────────────────────

    public string SaveDateTime;
}