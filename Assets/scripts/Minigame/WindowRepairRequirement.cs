using UnityEngine;

[RequireComponent(typeof(WindowInteractable))]
public class WindowRepairRequirement : MonoBehaviour
{
    [Header("Required Items (all three must be present)")]
    public ItemData requiredItem1;
    public ItemData requiredItem2;
    public ItemData requiredItem3;

    private WindowInteractable window;

    private void Awake()
    {
        window = GetComponent<WindowInteractable>();
    }

    public bool HasRequiredItems(Inventory inventory)
    {
        if (inventory == null) return false;

        bool has1 = requiredItem1 == null || inventory.CountOf(requiredItem1) >= 2;
        bool has2 = requiredItem2 == null || inventory.CountOf(requiredItem2) >= 4;
        bool has3 = requiredItem3 == null || inventory.CountOf(requiredItem3) >= 1;

        return has1 && has2 && has3;
    }

    public void ConsumeItems(Inventory inventory)
    {
        if (requiredItem1 != null) inventory.RemoveItem(requiredItem1, 2);
        if (requiredItem2 != null) inventory.RemoveItem(requiredItem2, 4);
       
    }
}