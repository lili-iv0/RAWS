using UnityEngine;

// Add to any trigger zone that transitions between areas.
// Tells the storm system whether the player is indoors or outdoors.
[RequireComponent(typeof(Collider2D))]
public class Zone : MonoBehaviour
{
    public bool isIndoor = false;

    [Tooltip("Default state when no zone has been entered yet (set true if player starts indoors)")]
    public static bool PlayerIsIndoor { get; private set; } = false;

    // Tracks which zone the player is currently in so exit logic is accurate
    private static Zone currentZone = null;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Start()
    {
        // On scene load, check if the player is already overlapping a zone
        var player = GameObject.FindWithTag("Player");
        if (player == null) return;

        var playerCol = player.GetComponent<Collider2D>();
        if (playerCol == null) return;

        var col = GetComponent<Collider2D>();
        if (col.bounds.Intersects(playerCol.bounds))
        {
            SetZone(this);
            Debug.Log($"[Zone] Player started inside zone: {gameObject.name} (indoor: {isIndoor})");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SetZone(this);
            Debug.Log($"[Zone] Player entered {(isIndoor ? "indoor" : "outdoor")} zone: {gameObject.name}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && currentZone == this)
        {
            // Player left this zone without entering another — assume outdoor
            PlayerIsIndoor = false;
            currentZone = null;
            Debug.Log($"[Zone] Player exited zone: {gameObject.name}, defaulting to outdoor.");
        }
    }

    private static void SetZone(Zone zone)
    {
        currentZone = zone;
        PlayerIsIndoor = zone.isIndoor;
    }
}