using UnityEngine;
using UnityEngine.SceneManagement;

public class WindowInteractable : MonoBehaviour, IInteractable
{
    public GameObject interactPrompt;
    public string windowID;

    private StormSystem stormSystem;
    private bool playerInRange = false;

    private void Start()
    {
        stormSystem = FindAnyObjectByType<StormSystem>();

        if (string.IsNullOrEmpty(windowID))
            Debug.LogWarning($"[WindowInteractable] windowID is not set on {gameObject.name}");
    }

    private void Update()
    {
        if (playerInRange)
            UpdatePromptVisibility();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        UpdatePromptVisibility();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        SetPrompt(false);
    }

    private void UpdatePromptVisibility()
    {
        bool shouldShow = !IsWindowFixed() && IsStorming();
        SetPrompt(shouldShow);
    }

    private void SetPrompt(bool active)
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(active);
    }

    public void Interact(Player player)
    {
        if (!IsStorming() || IsWindowFixed()) return;

        var requirement = GetComponent<WindowRepairRequirement>();
        if (requirement != null)
        {
            Inventory inventory = player.GetComponent<Inventory>();
            if (inventory == null)
            {
                Debug.LogWarning("[Window] Player has no Inventory component.");
                return;
            }
            if (!requirement.HasRequiredItems(inventory))
            {
                Debug.Log("[Window] Missing required repair items.");
                return;
            }
            requirement.ConsumeItems(inventory);
        }

        SavePlayerState(player);
        SceneManager.LoadScene("MinigameScene");
        QuestManager.Instance?.NotifyObjectInteracted(windowID);
    }

    private void SavePlayerState(Player player)
    {
        if (GameData.Instance == null) return;

        GameData.Instance.playerPosition = player.transform.position;
        GameData.Instance.hasPosition = true;
        GameData.Instance.currentWindowID = windowID;

        PlayerStatus ps = player.GetComponent<PlayerStatus>();
        if (ps != null)
        {
            GameData.Instance.playerHealth = ps.health;
            GameData.Instance.playerHunger = ps.hunger;
            GameData.Instance.playerThirst = ps.thirst;
        }
    }

    private bool IsWindowFixed()
        => GameData.Instance != null && GameData.Instance.IsWindowFixed(windowID);

    private bool IsStorming()
        => stormSystem != null && stormSystem.IsStorming;
}