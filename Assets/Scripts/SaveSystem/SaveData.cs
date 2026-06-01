using System;
using System.Collections.Generic;   // ← add this

[Serializable]
public class SaveData
{
    public float PlayerX;
    public float PlayerY;
    public float RotationZ;

    public float Health;
    public float Stamina;
    public int   Score;

    public string LastCheckpointName;
    public string SaveDateTime;

    public List<string> DisabledDialogueZones = new List<string>();
    public List<string> DefeatedEncounterIDs  = new List<string>();
}