using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance { get; private set; }
    
    public List<PlayerEntity> Members { get; private set; } = new();
    public List<BaseCombatItem> Inventory { get; private set; } = new();
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RestoreAllPartyMembersStats()
    {
        foreach (var member in Members)
        {
            member.RestoreStats();
        }
    }

    public void CheckForDeadMembers()
    {
        Members.RemoveAll(member => member.CurrentHp <= 0);
        print("Removed from party. Remaining members: " + PartyManager.Instance.Members.Count);
    }
}