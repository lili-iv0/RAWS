using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    public Image background;
    public Image icon;
    public TextMeshProUGUI quantityLabel;

    [Header("Colors")]
    public Color normalColor ;
    public Color selectedColor  ;
    public Color emptyColor   ;
    private void Reset()
    {
        normalColor = new Color(0.15f, 0.15f, 0.15f, 0.9f);
        selectedColor = new Color(0.9f, 0.7f, 0.1f, 1f);
        emptyColor = new Color(0.1f, 0.1f, 0.1f, 0.6f);
    }

    // Which inventory and slot this UI cell represents
    public Inventory linkedInventory { get; private set; }
    public int slotIndex             { get; private set; }
    private InventoryUI parentUI;

    public void Setup(Inventory inventory, int index, InventoryUI ui)
    {
        linkedInventory = inventory;
        slotIndex       = index;
        parentUI        = ui;
        Refresh();
    }

    public void Refresh()
    {
        
        if (linkedInventory == null || slotIndex >= linkedInventory.slots.Count) return;

        var slot = linkedInventory.slots[slotIndex];

        if (slot.IsEmpty)
        {
            icon.enabled         = false;
            quantityLabel.text   = string.Empty;
            background.color     = emptyColor;
        }
        else
        {
            icon.enabled         = true;
            icon.sprite          = slot.item.icon;
            quantityLabel.text   = slot.quantity > 1 ? slot.quantity.ToString() : string.Empty;
            background.color     = normalColor;
        }
    }

    public void SetSelected(bool selected)
    {
        var slot = linkedInventory?.slots[slotIndex];
        if (slot == null) return;

        background.color = selected
            ? selectedColor
            : (slot.IsEmpty ? emptyColor : normalColor);
    }

    // Left-click → select this slot in the parent UI
    public void OnPointerClick(PointerEventData eventData)
    {
        parentUI?.OnSlotClicked(this);
    }
}
