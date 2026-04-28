using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCombatItem : BaseAction
{
    public int Quantity { get; private set; }

    protected BaseCombatItem(string name, int targetCount, bool targetEnemy, int quantity, int vfxNumber) : base(name, targetCount, targetEnemy, vfxNumber)
    {
        this.Quantity = quantity;
    }

    public override IEnumerator Apply(BaseEntity source, List<BaseEntity> targets)
    {
        if (Quantity <= 0)
        {
            Debug.LogError($"Tried to use {Name} but quantity is {Quantity}. Removing from inventory.");
            RemoveFromInventory();
            yield break;
        }
        
        yield return base.Apply(source, targets);
        
        Quantity--;
        if (Quantity <= 0)
        {
            RemoveFromInventory();
        }
    }

    public bool IncreaseQuantity(int amount)
    {
        Quantity += amount;
        return true;
    }
    
    private void RemoveFromInventory()
    {
        var inveontoryItem = PartyManager.Instance.Inventory.Find(i => i == this);
        if (inveontoryItem != null)
        {
            PartyManager.Instance.Inventory.Remove(inveontoryItem);
        }
    }
}
