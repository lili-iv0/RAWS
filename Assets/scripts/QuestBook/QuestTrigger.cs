using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    [Header("Quest to Add")]
    public QuestData quest;

    [Header("Trigger Mode")]
    public TriggerMode mode = TriggerMode.Auto;

    [Header("One Shot")]
    public bool oneShot = true;

    [Header("Storm Required")]
    [Tooltip("If true, quest only adds when storm is active")]
    public bool requiresStorm = false;

    private bool hasFired = false;
    private StormSystem stormSystem;

    public enum TriggerMode { Auto, Manual }

    private void Start()
    {
        stormSystem = FindAnyObjectByType<StormSystem>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (mode == TriggerMode.Auto && other.CompareTag("Player")) TryAddQuest();
    }

    public void Interact(Player player)
    {
        if (mode == TriggerMode.Manual) TryAddQuest();
    }

    private void TryAddQuest()
    {
        if (hasFired && oneShot) return;
        if (quest == null || QuestManager.Instance == null) return;
        if (requiresStorm && (stormSystem == null || !stormSystem.IsStorming)) return;

        QuestManager.Instance.AddQuest(quest);
        if (oneShot) hasFired = true;
    }
}