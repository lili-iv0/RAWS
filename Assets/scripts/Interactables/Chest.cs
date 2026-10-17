using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Inventory))]
public class Chest : MonoBehaviour, IInteractable
{
    [Header("Starting Loot")]
    [Tooltip("Items placed in this chest when the scene loads.")]
    public List<StartingLoot> startingLoot = new List<StartingLoot>();

    [Header("Settings")]
    public string chestTitle = "Chest";
    public ChestUI chestUI;
    [Header("Optional")]
    public Animator animator;
    public AudioClip openSound;
    public AudioClip closeSound;
    private AudioSource audioSource;

    private Inventory chestInventory;
    private bool isOpen = false;


    private Player currentPlayer;
    [System.Serializable]
    public class StartingLoot
    {
        public ItemData item;
        public int quantity = 1;
    }

    //Lifecycle

    private void Awake()
    {
        chestInventory = GetComponent<Inventory>();
        audioSource    = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Populate chest with starting loot
        foreach (var entry in startingLoot)
            if (entry.item != null)
                chestInventory.AddItem(entry.item, entry.quantity);
    }

    //IInteractable 

    public void Interact(Player player)
    {
        if (isOpen)
            Close(player);
        else
            Open(player);
    }

    //Open / Close

    private void Open(Player player)
    {
        currentPlayer = player;
        isOpen = true;

        if (animator != null) animator.SetTrigger("Open");
        PlaySound(openSound);

        // Find ChestUI in scene and open it
        if (chestUI != null)
        {
            Inventory playerInventory = player.GetComponent<Inventory>();
            chestUI.Open(chestInventory, playerInventory, chestTitle);
        }
        else
        {
            Debug.LogWarning("[Chest] No ChestUI found in scene.");
        }

        Debug.Log($"[Chest] {gameObject.name} opened.");
    }

    private void Close(Player player)
    {
        currentPlayer = null;
        isOpen = false;

        if (animator != null) animator.SetTrigger("Close");
        PlaySound(closeSound);

        ChestUI ui = FindAnyObjectByType<ChestUI>();
        ui?.Close();

        Debug.Log($"[Chest] {gameObject.name} closed.");
    }



    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (isOpen && other.CompareTag("Player"))
            Close(currentPlayer);
    }
}
