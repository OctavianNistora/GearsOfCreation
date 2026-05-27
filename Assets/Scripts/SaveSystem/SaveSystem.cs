using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Static helper — handles all save file disk I/O.
/// </summary>
public static class SaveSystem
{
    private static readonly string SavePath =
        Path.Combine(Application.persistentDataPath, "save.json");

    // ─────────────────────────────────────────────────────────────────────────
    // SAVE
    // ─────────────────────────────────────────────────────────────────────────

    public static void Save(SaveData data)
    {
        try
        {
            data.SaveDateTime =
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string json =
                JsonUtility.ToJson(data, prettyPrint: true);

            File.WriteAllText(SavePath, json);

            Debug.Log($"[SaveSystem] Saved → {SavePath}");
        }
        catch (Exception e)
        {
            Debug.LogError(
                $"[SaveSystem] Save failed: {e.Message}");
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // LOAD
    // ─────────────────────────────────────────────────────────────────────────

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

            SaveData data =
                JsonUtility.FromJson<SaveData>(json);

            Debug.Log(
                $"[SaveSystem] Loaded save from {data.SaveDateTime}");

            return data;
        }
        catch (Exception e)
        {
            Debug.LogError(
                $"[SaveSystem] Load failed: {e.Message}");

            return null;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // HELPERS
    // ─────────────────────────────────────────────────────────────────────────

    public static bool HasSave()
    {
        return File.Exists(SavePath);
    }

    public static void DeleteSave()
    {
        if (!HasSave())
            return;

        File.Delete(SavePath);

        Debug.Log("[SaveSystem] Save deleted.");
    }
}