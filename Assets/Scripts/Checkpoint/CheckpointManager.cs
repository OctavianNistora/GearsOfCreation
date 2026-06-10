using System;
using System.Collections.Generic;
using UnityEngine;
using DefaultNamespace;
using System.Threading.Tasks;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private PlayerMemento _latestMemento;

    public bool[] activeCheckpoints = new bool[6];
    public  CheckpointTrigger LastCheckpoint { get; private set; }
    public  bool HasCheckpoint() => _latestMemento != null;

    private void Awake()
    {
        // If an old instance exists (from DontDestroyOnLoad in editor),
        // destroy the OLD one and replace it with this fresh one.
        // This ensures the latest compiled code is always running.
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (SaveSystem.HasSave())
            LoadGame();
        activeCheckpoints = new bool[6]; // reset active checkpoints on start
    }

    public void SetNewMementoPosition(Vector3 newPosition)
    {
        if (_latestMemento == null) return;
        _latestMemento = new PlayerMemento(
            position:        newPosition,
            rotation:        _latestMemento.Rotation,
            velocity:        _latestMemento.Velocity,
            angularVelocity: _latestMemento.AngularVelocity,
            health:          _latestMemento.Health,
            stamina:         _latestMemento.Stamina,
            score:           _latestMemento.Score
        );
        SaveGame();
    }

    public void SaveCheckpoint(PlayerOriginator player, CheckpointTrigger trigger)
    {
        if (player == null || trigger == null)
        {
            Debug.LogWarning("[Checkpoint] Save failed: missing player or trigger.");
            return;
        }

        // Destroy the collider on the previous checkpoint permanently
        if (LastCheckpoint != null)
        {
            BoxCollider2D prev = LastCheckpoint.GetComponent<BoxCollider2D>();
            if (prev != null)
            {
                Destroy(prev);
                Debug.Log("[Checkpoint] Destroyed BoxCollider2D on '" + LastCheckpoint.name + "'.");
            }
            else
            {
                Debug.LogError("[Checkpoint] '" + LastCheckpoint.name + "' has no BoxCollider2D to destroy!");
            }
        }
        else
        {
            Debug.Log("[Checkpoint] First checkpoint reached — no previous to destroy.");
        }

        _latestMemento = player.SaveState();
        LastCheckpoint  = trigger;
        Debug.Log("[Checkpoint] Active checkpoint is now '" + trigger.name + "'.");

        SaveGame();
    }

    public void RestoreLastCheckpoint(PlayerOriginator player)
    {
        if (_latestMemento == null) { Debug.LogWarning("[Checkpoint] No checkpoint saved yet."); return; }
        if (player == null) { Debug.LogWarning("[Checkpoint] Restore failed: player is null."); return; }
        player.RestoreState(_latestMemento);
    }

    public void SaveGame()
    {
        if (_latestMemento == null) { Debug.LogWarning("[SaveSystem] No memento to save."); return; }

        SaveData data = new SaveData
        {
            PlayerX            = _latestMemento.Position.x,
            PlayerY            = _latestMemento.Position.y,
            RotationZ          = _latestMemento.Rotation.eulerAngles.z,
            Health             = _latestMemento.Health,
            Stamina            = _latestMemento.Stamina,
            Score              = _latestMemento.Score,
            LastCheckpointName = LastCheckpoint != null ? LastCheckpoint.name : string.Empty,
        };

        // --- SALVARE STARE DIALOGURI ---
        foreach (var zone in FindObjectsByType<DialogueZone>(FindObjectsSortMode.None))
        {
            if (zone.IsDisabled)
            {
                data.DisabledDialogueZones.Add(zone.gameObject.name);
            }
        }

        // --- SALVARE STARE ENCOUNTERS VIA REFLECTION ---
        foreach (var encounter in FindObjectsByType<Encounter>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (!encounter.gameObject.activeSelf)
            {
                string encounterID = GetEncounterIDViaReflection(encounter);
                if (!string.IsNullOrEmpty(encounterID))
                {
                    data.DefeatedEncounterIDs.Add(encounterID);
                }
            }
        }

        SaveSystem.Save(data);
    }

    public void LoadGame()
    {
        SaveData data = SaveSystem.Load();
        if (data == null) return;

        foreach (var cp in FindObjectsByType<CheckpointTrigger>(FindObjectsSortMode.None))
        {
            if (cp.name == data.LastCheckpointName)
                LastCheckpoint = cp;
        }

        // --- RESTAURARE DIALOGURI ---
        foreach (var zone in FindObjectsByType<DialogueZone>(FindObjectsSortMode.None))
        {
            if (data.DisabledDialogueZones.Contains(zone.gameObject.name))
            {
                zone.ForceDisable();
            }
            else
            {
                var collider = zone.GetComponent<BoxCollider2D>();
                if (collider != null) collider.enabled = true;
            }
        }

        // --- RESTAURARE ENCOUNTERS ---
        foreach (var encounter in FindObjectsByType<Encounter>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            string encounterID = GetEncounterIDViaReflection(encounter);
            if (string.IsNullOrEmpty(encounterID)) continue;

            if (data.DefeatedEncounterIDs.Contains(encounterID))
            {
                encounter.gameObject.SetActive(false);
            }
            else
            {
                encounter.gameObject.SetActive(true);
            }
        }

        _latestMemento = new PlayerMemento(
            position:        new Vector3(data.PlayerX, data.PlayerY, 0f),
            rotation:        Quaternion.Euler(0f, 0f, data.RotationZ),
            velocity:        Vector3.zero,
            angularVelocity: Vector3.zero,
            health:          data.Health,
            stamina:         data.Stamina,
            score:           data.Score
        );

        PlayerOriginator player = FindFirstObjectByType<PlayerOriginator>();
        if (player == null) { Debug.LogError("[SaveSystem] PlayerOriginator not found!"); return; }
        player.RestoreState(_latestMemento);
        Debug.Log("[SaveSystem] Game loaded. Player at (" + data.PlayerX + ", " + data.PlayerY + ")");
    }

    // Rutină ajutătoare de Reflection pentru a extrage valoarea din câmpul privat/protejat '_id' moștenit din MonoBehaviourID
    private string GetEncounterIDViaReflection(Encounter encounter)
    {
        try
        {
            // Căutăm câmpul numit "_id" în ierarhia claselor (deoarece aparține clasei părinte MonoBehaviourID)
            var field = typeof(Encounter).GetField("_id", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy);
            if (field == null)
            {
                // Dacă nu-l găsește direct, verificăm tipul de bază explicit
                field = typeof(Encounter).BaseType?.GetField("_id", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            }

            if (field != null)
            {
                var idObj = field.GetValue(encounter);
                if (idObj != null)
                {
                    // Accesăm proprietatea .Value a obiectului ID (presupunând că ID-ul are proprietatea publică .Value de tip string)
                    var valueProperty = idObj.GetType().GetProperty("Value");
                    if (valueProperty != null)
                    {
                        return valueProperty.GetValue(idObj)?.ToString();
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[CheckpointManager] Reflection failed to get ID: " + e.Message);
        }
        return string.Empty;
    }

    private void OnApplicationQuit()
    {
#if UNITY_EDITOR
        SaveSystem.DeleteSave();
#endif
    }
}