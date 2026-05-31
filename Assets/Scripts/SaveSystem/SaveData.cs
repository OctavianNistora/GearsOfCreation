using System;

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
}