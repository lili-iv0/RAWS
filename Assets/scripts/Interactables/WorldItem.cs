using UnityEngine;

// Attach to any world pickup (wood, dropped item, etc.)
// Requires a SpriteRenderer. Set the GameObject layer to Interactable.
[RequireComponent(typeof(SpriteRenderer))]
public class WorldItem : MonoBehaviour, IInteractable
{
    [Header("Item")]
    public ItemData item;
    public int quantity = 1;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (item != null) sr.sprite = item.icon;
    }

    //IInteractable 

    public void Interact(Player player)
    {
        Inventory inventory = player.GetComponent<Inventory>();
        if (inventory == null)
        {
            Debug.LogWarning("[WorldItem] No Inventory on Player.");
            return;
        }

        if (!inventory.HasSpaceFor(item, quantity))
        {
            Debug.Log($"[WorldItem] Inventory full — {item.displayName} stays in world.");
            return;     // Item remains in the world
        }

        inventory.AddItem(item, quantity);
        Debug.Log($"[WorldItem] Picked up {quantity}x {item.displayName}.");

        // Item is consumed — remove from world
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
