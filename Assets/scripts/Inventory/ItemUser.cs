using UnityEngine;

public class ItemUser : MonoBehaviour
{
    private Inventory inventory;
    private PlayerStatus status;
    public InventoryUI inventoryUI;

    void Start()
    {

        inventory = GetComponent<Inventory>();
        status = GetComponent<PlayerStatus>();
      
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            UseSelected();
    }

    public void UseSelected()
    {

        Debug.Log("F pressed, UseSelected called");

        if (inventoryUI.selectedSlot == null) { Debug.Log("no slot selected"); return; }

        var slot = inventory.slots[inventoryUI.selectedSlot.slotIndex];
        Debug.Log($"slot item: {slot.item}, isConsumable: {slot.item?.isConsumable}");

        if (slot.IsEmpty || !slot.item.isConsumable) { Debug.Log("slot empty or not consumable"); return; }


        if (inventoryUI.selectedSlot == null) return;
        _ = inventory.slots[inventoryUI.selectedSlot.slotIndex];
        if (slot.IsEmpty || !slot.item.isConsumable) return;

        switch (slot.item.consumeType)
        {
            case ItemData.ConsumeType.Food:
                status.Eat(slot.item.consumeAmount);
                break;
            case ItemData.ConsumeType.Drink:
                status.Drink(slot.item.consumeAmount);
                break;
            case ItemData.ConsumeType.Health:
                status.TakeDamage(-slot.item.consumeAmount);
                break;
        }

        inventory.RemoveFromSlot(inventoryUI.selectedSlot.slotIndex, 1);
    }
}