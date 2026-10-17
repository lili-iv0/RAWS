using UnityEngine;

// Create via: Right-click in Project → Create → Inventory → Item
// One asset per item type. Drag into WorldItem or Chest loot lists.
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    public string itemID;           // Unique key — must match across all systems
    public string displayName;
    [TextArea] public string description;

    [Header("Visuals")]
    public Sprite icon;             // Shown in inventory/chest UI slots

    [Header("Stacking")]
    public bool stackable = true;
    public int maxStack = 16;

    [Header("Consumable")]
    public bool isConsumable = false;
    public enum ConsumeType { Food, Drink, Health }
    public ConsumeType consumeType; // What it restores when consumed
    public float consumeAmount = 15f; //ammount of hunger/thirst/health restored when consumed


}
