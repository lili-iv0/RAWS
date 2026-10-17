using UnityEngine;

[RequireComponent(typeof(AtticLadder))]
public class LockableLadder : MonoBehaviour
{
    [Header("Key Item")]
    public ItemData requiredKey;

    [Header("State")]
    public bool isLocked = true;

    [Header("UI")]
    public GameObject atticKeyCanvas;
    public GameObject controlCanvas;
    public GameObject bagButton;
    public GameObject interactButton;  
    private AtticLadder ladder;

    [Header("Quest")]
    public string objectID = "unlock_attic";

    private void Awake()
    {
        ladder = GetComponent<AtticLadder>();
        ladder.enabled = !isLocked;
        
        if (atticKeyCanvas != null)
            atticKeyCanvas.SetActive(false);
    }

    public void TryUnlock(Player player)
    {
        if (!isLocked) return;

        Inventory inventory = player.GetComponent<Inventory>();
        if (inventory == null) return;

        if (inventory.CountOf(requiredKey) <= 0)
        {
            if (controlCanvas != null)
                controlCanvas.SetActive(false);
            if (bagButton != null)
                bagButton.SetActive(false);
            if (interactButton != null)
                interactButton.SetActive(false); 
            if (atticKeyCanvas != null)
            {
                atticKeyCanvas.SetActive(true);
                Invoke("HideUI", 2f);
            }
            Debug.Log($"[LockableLadder] Need {requiredKey.displayName} to unlock.");
            return;
        }

        inventory.RemoveItem(requiredKey, 1);
        isLocked = false;
        ladder.enabled = true;
        QuestManager.Instance?.NotifyObjectInteracted(objectID);
        if (atticKeyCanvas != null)
            atticKeyCanvas.SetActive(false);
            
        Debug.Log($"[LockableLadder] Unlocked with {requiredKey.displayName}.");
    }
    
    private void HideUI()
    {
        if (atticKeyCanvas != null)
            atticKeyCanvas.SetActive(false);
        if (controlCanvas != null)
            controlCanvas.SetActive(true);
        if (bagButton != null)
            bagButton.SetActive(true);
        if (interactButton != null)
            interactButton.SetActive(true); 
    }
}