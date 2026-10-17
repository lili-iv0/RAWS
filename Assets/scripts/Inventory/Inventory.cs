using System.Collections.Generic;
using UnityEngine;

// ── Inventory Slot ────────────────────────────────────────────────────────────
[System.Serializable]
public class InventorySlot
{
    public ItemData item;       // null = empty slot
    public int quantity;

    public InventorySlot() { item = null; quantity = 0; }

    public bool IsEmpty => item == null || quantity == 0;
    public bool IsFull  => item != null && quantity >= item.maxStack;

    public void Clear() { item = null; quantity = 0; }
}

//Inventory 
// Attach to Player, Chest, or any storage object. (universal container for items isa nalang para sa lahat ng storage)
public class Inventory : MonoBehaviour
{
    [Header("Settings")]
    public int size = 6;

    public List<InventorySlot> slots { get; private set; }

    // Fired whenever the inventory changes — UI listens to this
    public event System.Action OnInventoryChanged;

    private void Awake() => Initialize();

    public void Initialize()
    {
        slots = new List<InventorySlot>(size);
        for (int i = 0; i < size; i++)
            slots.Add(new InventorySlot());
    }

    //Queries

    public bool HasSpaceFor(ItemData item, int amount = 1)
    {
        int remaining = amount;

        // Space in existing stacks
        foreach (var slot in slots)
            if (!slot.IsEmpty && slot.item == item)
                remaining -= (item.maxStack - slot.quantity);

        if (remaining <= 0) return true;

        // Empty slots
        foreach (var slot in slots)
            if (slot.IsEmpty) return true;

        return false;
    }

    public int CountOf(ItemData item)
    {
        int total = 0;
        foreach (var slot in slots)
            if (!slot.IsEmpty && slot.item == item)
                total += slot.quantity;
        return total;
    }

    // Mutations

    public bool AddItem(ItemData item, int amount = 1)
    {
        if (!HasSpaceFor(item, amount)) return false;

        int remaining = amount;

        // Fill existing stacks first
        foreach (var slot in slots)
        {
            if (remaining <= 0) break;
            if (slot.IsEmpty || slot.item != item) continue;

            int room = item.maxStack - slot.quantity;
            int toAdd = Mathf.Min(room, remaining);
            slot.quantity += toAdd;
            remaining -= toAdd;
        }

        // Open new slots for the rest
        foreach (var slot in slots)
        {
            if (remaining <= 0) break;
            if (!slot.IsEmpty) continue;

            slot.item = item;
            slot.quantity = Mathf.Min(item.maxStack, remaining);
            remaining -= slot.quantity;
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (CountOf(item) < amount) return false;

        int remaining = amount;
        foreach (var slot in slots)
        {
            if (remaining <= 0) break;
            if (slot.IsEmpty || slot.item != item) continue;

            int toRemove = Mathf.Min(slot.quantity, remaining);
            slot.quantity -= toRemove;
            remaining -= toRemove;
            if (slot.quantity == 0) slot.Clear();
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    // Remove exactly one item from a specific slot index (used by UI transfers)
    public bool RemoveFromSlot(int slotIndex, int amount = 1)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return false;
        var slot = slots[slotIndex];
        if (slot.IsEmpty || slot.quantity < amount) return false;

        slot.quantity -= amount;
        if (slot.quantity == 0) slot.Clear();

        OnInventoryChanged?.Invoke();
        return true;
    }

    [ContextMenu("Debug Print")]
    public void DebugPrint()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            var s = slots[i];
            if (!s.IsEmpty)
                Debug.Log($"  [{i}] {s.item.displayName} x{s.quantity}");
        }
    }
}
