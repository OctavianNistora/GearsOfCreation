using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Handles all disk I/O for the save system.
/// 
/// USAGE:
///   SaveSystem.Save(saveData)      — write to disk
///   SaveSystem.Load()              — read from disk, returns null if no save exists
///   SaveSystem.HasSave()           — check if a save file exists
///   SaveSystem.DeleteSave()        — wipe the save (e.g. "New Game" button)
/// 
/// Save file location: Application.persistentDataPath/save.json
/// On Windows: %APPDATA%/../LocalLow/<Company>/<Product>/save.json
/// On Mac:     ~/Library/Application Support/<Company>/<Product>/save.json
/// </summary>
public static class SaveSystem
{
    private static readonly string SavePath =
        Path.Combine(Application.persistentDataPath, "save.json");

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    public static void Save(SaveData data)
    {
        try
        {
            data.SaveDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"[SaveSystem] Game saved to: {SavePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to save: {e.Message}");
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
            string json = File.ReadAllText(SavePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log($"[SaveSystem] Loaded save from {data.SaveDateTime}");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to load save: {e.Message}");
            return null;
        }
    }

    public static bool HasSave() => File.Exists(SavePath);

    public static void DeleteSave()
    {
        if (HasSave())
        {
            File.Delete(SavePath);
            Debug.Log("[SaveSystem] Save file deleted.");
        }
    }
}