using System;
using System.Collections.Generic;
using UnityEngine;
using DefaultNamespace;
using System.Threading.Tasks;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    // 🔥 used to prevent auto-load after victory scene transition
    public static bool SkipAutoLoadOnce;

    private PlayerMemento _latestMemento;
    public CheckpointTrigger LastCheckpoint { get; private set; }

    public bool HasCheckpoint() => _latestMemento != null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (SkipAutoLoadOnce)
        {
            SkipAutoLoadOnce = false;
            return;
        }

        if (SaveSystem.HasSave())
        {
            LoadGame();
        }
    }

    public void SaveCheckpoint(PlayerOriginator player, CheckpointTrigger trigger)
    {
        if (player == null || trigger == null)
        {
            Debug.LogWarning("[Checkpoint] Save failed: null reference.");
            return;
        }

        _latestMemento = player.SaveState();
        LastCheckpoint = trigger;

        SaveGame();
    }

    public void RestoreLastCheckpoint(PlayerOriginator player)
    {
        if (_latestMemento == null || player == null)
        {
            Debug.LogWarning("[Checkpoint] Restore failed.");
            return;
        }

        player.RestoreState(_latestMemento);
    }

    public void SaveGame()
    {
        if (_latestMemento == null)
        {
            Debug.LogWarning("[Checkpoint] No data to save.");
            return;
        }

        SaveData data = new SaveData
        {
            PlayerX = _latestMemento.Position.x,
            PlayerY = _latestMemento.Position.y,
            RotationZ = _latestMemento.Rotation.eulerAngles.z,
            Health = _latestMemento.Health,
            Stamina = _latestMemento.Stamina,
            Score = _latestMemento.Score,
            LastCheckpointName = LastCheckpoint != null ? LastCheckpoint.name : string.Empty
        };

        // ----------------------------
        // DIALOGUE STATE
        // ----------------------------
        foreach (var zone in FindObjectsByType<DialogueZone>(FindObjectsSortMode.None))
        {
            if (zone.IsDisabled)
                data.DisabledDialogueZones.Add(zone.gameObject.name);
        }

        // ----------------------------
        // ENCOUNTER SAVE (ONLY VALID ONES)
        // ----------------------------
        float checkpointX = LastCheckpoint != null
            ? LastCheckpoint.transform.position.x
            : float.MaxValue;

        foreach (var encounter in FindObjectsByType<Encounter>(
                     FindObjectsInactive.Include,
                     FindObjectsSortMode.None))
        {
            if (!encounter.gameObject.activeSelf &&
                encounter.transform.position.x <= checkpointX)
            {
                string id = GetEncounterID(encounter);

                if (!string.IsNullOrEmpty(id))
                    data.DefeatedEncounterIDs.Add(id);
            }
        }

        SaveSystem.Save(data);
    }

    public void LoadGame()
    {
        SaveData data = SaveSystem.Load();
        if (data == null) return;

        // ----------------------------
        // CHECKPOINT RESTORE
        // ----------------------------
        foreach (var cp in FindObjectsByType<CheckpointTrigger>(FindObjectsSortMode.None))
        {
            if (cp.name == data.LastCheckpointName)
                LastCheckpoint = cp;
        }

        // ----------------------------
        // DIALOGUE RESTORE
        // ----------------------------
        foreach (var zone in FindObjectsByType<DialogueZone>(FindObjectsSortMode.None))
        {
            if (data.DisabledDialogueZones.Contains(zone.gameObject.name))
                zone.ForceDisable();
            else
            {
                var col = zone.GetComponent<BoxCollider2D>();
                if (col != null)
                    col.enabled = true;
            }
        }

        // =====================================================
        // 🔥 CRITICAL FIX: RESET ALL ENCOUNTERS FIRST
        // =====================================================
        foreach (var encounter in FindObjectsByType<Encounter>(
                     FindObjectsInactive.Include,
                     FindObjectsSortMode.None))
        {
            encounter.gameObject.SetActive(true);
        }

        // sync runtime memory
        if (EncounterProgressManager.Instance != null)
        {
            EncounterProgressManager.Instance.ResetDefeatedEncounters(
                data.DefeatedEncounterIDs
            );
        }

        // apply save state
        foreach (var encounter in FindObjectsByType<Encounter>(
                     FindObjectsInactive.Include,
                     FindObjectsSortMode.None))
        {
            string id = GetEncounterID(encounter);
            if (string.IsNullOrEmpty(id)) continue;

            if (data.DefeatedEncounterIDs.Contains(id))
                encounter.gameObject.SetActive(false);
        }

        // ----------------------------
        // PLAYER RESTORE
        // ----------------------------
        _latestMemento = new PlayerMemento(
            position: new Vector3(data.PlayerX, data.PlayerY, 0f),
            rotation: Quaternion.Euler(0f, 0f, data.RotationZ),
            velocity: Vector3.zero,
            angularVelocity: Vector3.zero,
            health: data.Health,
            stamina: data.Stamina,
            score: data.Score
        );

        PlayerOriginator player = FindFirstObjectByType<PlayerOriginator>();

        if (player == null)
        {
            Debug.LogError("[Checkpoint] Player not found.");
            return;
        }

        player.RestoreState(_latestMemento);

        Debug.Log("[Checkpoint] Load complete.");
    }

    // ----------------------------
    // SAFE ENCOUNTER ID READ
    // ----------------------------
    private string GetEncounterID(Encounter encounter)
    {
        try
        {
            var field = typeof(Encounter).GetField(
                "_id",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.FlattenHierarchy
            );

            var idObj = field?.GetValue(encounter);
            var prop = idObj?.GetType().GetProperty("Value");

            return prop?.GetValue(idObj)?.ToString();
        }
        catch
        {
            return string.Empty;
        }
    }

    private void OnApplicationQuit()
    {
#if UNITY_EDITOR
        SaveSystem.DeleteSave();
#endif
    }
}