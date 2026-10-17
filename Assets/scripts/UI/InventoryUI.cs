using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Attach to the root panel of an inventory/chest UI.
// Assign slotPrefab and slotGrid in Inspector.
// Call Bind(inventory) to connect to any Inventory component.
public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public GameObject slotPrefab;       // Prefab with InventorySlotUI component
    public Transform slotGrid;          // GridLayoutGroup parent

    [Header("Item Info Panel")]
    public GameObject infoPanel;        // Panel that shows selected item detail
    public Image infoIcon;
    public TextMeshProUGUI infoName;
    public TextMeshProUGUI infoDescription;
    public TextMeshProUGUI infoQuantity;

    // Currently bound inventory
    public Inventory linkedInventory { get; private set; }

    // Currently selected slot (null if none)
    public InventorySlotUI selectedSlot { get; private set; }

    // Fired when selection changes — ChestUI listens to this
    public event System.Action<InventorySlotUI> OnSelectionChanged;
    public event System.Action<InventorySlotUI> OnDropRequested;
    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();

    //Binding 

    public void Bind(Inventory inventory)
    {
 
        if (linkedInventory != null)
            linkedInventory.OnInventoryChanged -= Refresh;

        linkedInventory = inventory;

        if (linkedInventory != null)
            linkedInventory.OnInventoryChanged += Refresh;

        BuildGrid();
        Refresh();
    }

    private void OnDestroy()
    {
        if (linkedInventory != null)
            linkedInventory.OnInventoryChanged -= Refresh;
    }

    // Grid Construction

    private void BuildGrid()
    {
        // Clear old slots
        foreach (Transform child in slotGrid)
            Destroy(child.gameObject);
        slotUIs.Clear();
        selectedSlot = null;

        if (linkedInventory == null) return;

        for (int i = 0; i < linkedInventory.slots.Count; i++)
        {
            var go   = Instantiate(slotPrefab, slotGrid);
            var slotUI = go.GetComponent<InventorySlotUI>();
            slotUI.Setup(linkedInventory, i, this);
            slotUIs.Add(slotUI);
        }
    }

    //Refresh

    public void Refresh()
    {
        foreach (var slotUI in slotUIs)
            slotUI.Refresh();

        UpdateInfoPanel();
    }

    //Selection

    // Called by InventorySlotUI on click
    public void OnSlotClicked(InventorySlotUI clicked)
    {
        // Deselect if clicking same slot again
        if (selectedSlot == clicked)
        {
            Deselect();
            return;
        }

        // Deselect previous
        selectedSlot?.SetSelected(false);

        // Only allow selecting non-empty slots
        var slot = clicked.linkedInventory.slots[clicked.slotIndex];
        if (slot.IsEmpty)
        {
            selectedSlot = null;
            UpdateInfoPanel();
            OnSelectionChanged?.Invoke(null);
            return;
        }

        selectedSlot = clicked;
        selectedSlot.SetSelected(true);
        UpdateInfoPanel();
        OnSelectionChanged?.Invoke(selectedSlot);
    }

    public void Deselect()
    {
        selectedSlot?.SetSelected(false);
        selectedSlot = null;
        UpdateInfoPanel();
        OnSelectionChanged?.Invoke(null);
    }

    void Update()
    { // Drop item with Q key
        if (Input.GetKeyDown(KeyCode.Q) && selectedSlot != null)
            OnDropRequested?.Invoke(selectedSlot);
    }

    //Info Panel

    private void UpdateInfoPanel()
    {
        Debug.Log($"UpdateInfoPanel called, selectedSlot: {selectedSlot}");
        if (infoPanel == null) return;

        if (selectedSlot == null)
        {
            infoPanel.SetActive(false);
            return;
        }

        var slot = selectedSlot.linkedInventory.slots[selectedSlot.slotIndex];
        Debug.Log($"slot empty: {slot.IsEmpty}, item: {slot.item}");

        if (slot.IsEmpty) { infoPanel.SetActive(false); return; }

        infoPanel.SetActive(true);
        if (infoIcon)        infoIcon.sprite       = slot.item.icon;
        if (infoName)        infoName.text         = slot.item.displayName;
        if (infoDescription) infoDescription.text  = slot.item.description;
        if (infoQuantity)    infoQuantity.text      = $"x{slot.quantity}";
    }

   
}
