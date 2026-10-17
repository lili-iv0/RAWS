using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject pauseCanvas;
    GameObject tempCanvas;
    public InventoryUI inventoryUI;
    public GameObject player;
    public WorldItem worldItemPrefab;
    public QuestManager questManager;
    public QuestBookUI questBookUI;
    public GameObject inventoryPanel;
    public GameObject questButton;
    public Button useItemButton;
  
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        var inv = GameObject.FindWithTag("Player").GetComponent<Inventory>();
        inventoryUI.Bind(inv);
        inventoryUI.OnDropRequested += DropItem;

        if (questBookUI != null && questManager != null)
            questBookUI.SetQuestManager(questManager);
        inventoryUI.OnSelectionChanged += OnSlotSelected;
        inv.OnInventoryChanged += UpdateQuestButton;
        UpdateQuestButton(); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);

        if (Input.GetKeyDown(KeyCode.Q) && questBookUI != null) { questBookUI.Toggle(); Debug.Log("Quest Book Toggled"); }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 0) resume();
            else pause();
        }
    }

    void DropItem(InventorySlotUI slot)
    {
        var inventorySlot = slot.linkedInventory.slots[slot.slotIndex];
        if (inventorySlot.IsEmpty) return;

        var dropped = Instantiate(worldItemPrefab, player.transform.position, Quaternion.identity);
        dropped.item = inventorySlot.item;
        dropped.quantity = inventorySlot.quantity;
        dropped.GetComponent<SpriteRenderer>().sprite = inventorySlot.item.icon;

        slot.linkedInventory.RemoveFromSlot(slot.slotIndex, inventorySlot.quantity);
    }

    public void pause()
    {
        tempCanvas = Instantiate(pauseCanvas);
        Time.timeScale = 0;
    }

    public void resume()
    {
        Destroy(tempCanvas);
        Time.timeScale = 1;
    }

    void OnSlotSelected(InventorySlotUI slot)
    {
        bool consumable = false;

        if (slot != null)
        {
            var item = slot.linkedInventory.slots[slot.slotIndex].item;
            consumable = item != null && item.isConsumable;
        }

        if (useItemButton != null) useItemButton.interactable = consumable;
      
    }
    public void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }
    public void OnQuestButton()
    {
        if (questBookUI != null)
            questBookUI.Toggle();
    }
    void UpdateQuestButton()
    {
        if (questButton == null) return;
        bool hasNotebook = questManager != null && questManager.HasNotebook();
        questButton.SetActive(hasNotebook);
    }
}
