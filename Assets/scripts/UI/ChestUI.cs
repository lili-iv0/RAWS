using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ChestUI : MonoBehaviour
{
    [Header("Panels")]
    public InventoryUI chestPanel;
    public InventoryUI playerPanel;

    [Header("Transfer Buttons")]
    public Button takeButton;
    public Button takeAllButton;
    public Button depositButton;
    public Button depositAllButton;

    [Header("Labels")]
    public TextMeshProUGUI chestTitle;

    [Header("Close")]
    public Button closeButton;

    [Header("UI to Hide")]
    public GameObject controllerUI;
    public GameObject interactionPromptUI;
    public GameObject bagUI;

    private Inventory chestInventory;
    private Inventory playerInventory;
    private InventorySlotUI chestSelected;
    private InventorySlotUI playerSelected;
    private PlayerMovement playerMovement;
    private void Awake()
    {
        takeButton.onClick.AddListener(TakeSelected);
        takeAllButton.onClick.AddListener(TakeAll);
        depositButton.onClick.AddListener(DepositSelected);
        depositAllButton.onClick.AddListener(DepositAll);
        closeButton.onClick.AddListener(Close);
        playerMovement = FindAnyObjectByType<PlayerMovement>();

        chestPanel.OnSelectionChanged += s => { chestSelected = s; RefreshButtons(); };
        playerPanel.OnSelectionChanged += s => { playerSelected = s; RefreshButtons(); };
    }

    private void Start()
    {
        chestPanel.gameObject.SetActive(true);
        playerPanel.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Open(Inventory chest, Inventory player, string title = "Chest")
    {
        chestInventory = chest;
        playerInventory = player;

        if (chestTitle != null) chestTitle.text = title;

        chestSelected = null;
        playerSelected = null;

        HideControllerUI();
        gameObject.SetActive(true);

        StartCoroutine(BindNextFrame(chest, player));
    }

    private IEnumerator BindNextFrame(Inventory chest, Inventory player)
    {
        yield return null;
        yield return null;
        chestPanel.Bind(chest);
        playerPanel.Bind(player);
        RefreshButtons();
    }

    public void Close()
    {
        chestPanel.Deselect();
        playerPanel.Deselect();
        
        ShowControllerUI();
        
        gameObject.SetActive(false);
    }

    private void HideControllerUI()
    {
        if (controllerUI != null) controllerUI.SetActive(false);
        if (interactionPromptUI != null) interactionPromptUI.SetActive(false);
        if (bagUI != null) bagUI.SetActive(false);
        if (playerMovement != null) playerMovement.enabled = false;
    }

    private void ShowControllerUI()
    {
        if (controllerUI != null) controllerUI.SetActive(true);
        if (interactionPromptUI != null) interactionPromptUI.SetActive(true);
        if (bagUI != null) bagUI.SetActive(true);
        if (playerMovement != null) playerMovement.enabled = true;
    }

    private void TakeSelected()
    {
        if (chestSelected == null) return;

        var slot = chestInventory.slots[chestSelected.slotIndex];
        if (slot.IsEmpty) return;

        int canTake = HowManyCanTransfer(slot.item, slot.quantity, chestInventory, playerInventory);
        if (canTake <= 0) { Debug.Log("[ChestUI] Player inventory full."); return; }

        playerInventory.AddItem(slot.item, canTake);
        chestInventory.RemoveFromSlot(chestSelected.slotIndex, canTake);

        chestPanel.Deselect();
    }

    private void TakeAll()
    {
        foreach (var slot in chestInventory.slots)
        {
            if (slot.IsEmpty) continue;
            int canTake = HowManyCanTransfer(slot.item, slot.quantity, chestInventory, playerInventory);
            if (canTake <= 0) continue;

            playerInventory.AddItem(slot.item, canTake);
            chestInventory.RemoveItem(slot.item, canTake);
        }

        chestPanel.Deselect();
    }

    private void DepositSelected()
    {
        if (playerSelected == null) return;

        var slot = playerInventory.slots[playerSelected.slotIndex];
        if (slot.IsEmpty) return;

        int canDeposit = HowManyCanTransfer(slot.item, slot.quantity, playerInventory, chestInventory);
        if (canDeposit <= 0) { Debug.Log("[ChestUI] Chest inventory full."); return; }

        chestInventory.AddItem(slot.item, canDeposit);
        playerInventory.RemoveFromSlot(playerSelected.slotIndex, canDeposit);

        playerPanel.Deselect();
    }

    private void DepositAll()
    {
        foreach (var slot in playerInventory.slots)
        {
            if (slot.IsEmpty) continue;
            int canDeposit = HowManyCanTransfer(slot.item, slot.quantity, playerInventory, chestInventory);
            if (canDeposit <= 0) continue;

            chestInventory.AddItem(slot.item, canDeposit);
            playerInventory.RemoveItem(slot.item, canDeposit);
        }

        playerPanel.Deselect();
    }

    private int HowManyCanTransfer(ItemData item, int amount, Inventory from, Inventory to)
    {
        int space = 0;

        foreach (var slot in to.slots)
            if (!slot.IsEmpty && slot.item == item)
                space += item.maxStack - slot.quantity;

        foreach (var slot in to.slots)
            if (slot.IsEmpty)
                space += item.maxStack;

        return Mathf.Min(amount, space);
    }

    private void RefreshButtons()
    {
        if (chestInventory == null || playerInventory == null) return;
        
        bool chestHasSelection = chestSelected != null && chestSelected.slotIndex < chestInventory.slots.Count && !chestInventory.slots[chestSelected.slotIndex].IsEmpty;
        bool playerHasSelection = playerSelected != null && playerSelected.slotIndex < playerInventory.slots.Count && !playerInventory.slots[playerSelected.slotIndex].IsEmpty;

        takeButton.interactable = chestHasSelection;
        depositButton.interactable = playerHasSelection;

        takeAllButton.interactable = AnyItems(chestInventory);
        depositAllButton.interactable = AnyItems(playerInventory);
    }

    private bool AnyItems(Inventory inv)
    {
        if (inv == null) return false;
        foreach (var slot in inv.slots)
            if (!slot.IsEmpty) return true;
        return false;
    }
}