using System;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SavePath =
        Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save(SaveData data)
    {
        try
        {
            data.SaveDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            File.WriteAllText(SavePath, JsonUtility.ToJson(data, prettyPrint: true));
            Debug.Log("[SaveSystem] Saved to: " + SavePath);
        }
        catch (Exception e)
        {
            Debug.LogError("[SaveSystem] Save failed: " + e.Message);
        }
    }

    public static SaveData Load()
    {
        if (!HasSave())
        {
            Debug.Log("[SaveSystem] No save file found.");
            return null;
        }
        try
        {
            SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
            Debug.Log("[SaveSystem] Loaded save from " + data.SaveDateTime);
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError("[SaveSystem] Load failed: " + e.Message);
            return null;
        }
    }

    public static bool HasSave() => File.Exists(SavePath);

    public static void DeleteSave()
    {
        if (!HasSave()) return;
        File.Delete(SavePath);
        Debug.Log("[SaveSystem] Save deleted.");
    }

    public static void ResetProgress()
    {
        File.Delete(SavePath);
        Debug.Log("[SaveSystem] Save deleted.");
    }
}