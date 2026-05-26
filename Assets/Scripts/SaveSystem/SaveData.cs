using System;
using System.Collections.Generic;

/// <summary>
/// Plain serializable container — everything that gets written to disk.
/// No Unity types allowed here (Vector3 etc. aren't serializable by default),
/// so we store primitives only and convert in SaveSystem.
/// </summary>
[Serializable]
public class SaveData
{
    // ── Player transform ──────────────────────────────────────────────────
    public float PlayerX;
    public float PlayerY;
    public float RotationZ;   // 2D only needs the Z rotation angle

    // ── Player stats ──────────────────────────────────────────────────────
    public float Health;
    public float Stamina;
    public int   Score;

    // ── Checkpoint ────────────────────────────────────────────────────────
    public string LastCheckpointName; // GameObject name of the last checkpoint

    // ── Meta ──────────────────────────────────────────────────────────────
    public string SaveDateTime; // human-readable, for debugging
}