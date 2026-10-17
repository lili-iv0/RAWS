using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Notebook Item Required")]
    public ItemData notebookItem;

    private Dictionary<string, QuestData> activeQuests = new Dictionary<string, QuestData>();
    private Dictionary<string, List<QuestStep>> questSteps = new Dictionary<string, List<QuestStep>>();

    public event System.Action<QuestData> OnQuestAdded;
    public event System.Action<QuestData> OnQuestCompleted;
    public event System.Action<QuestData, QuestStep> OnStepCompleted;

    private Inventory playerInventory;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null) playerInventory = player.GetComponent<Inventory>();
        if (playerInventory != null) playerInventory.OnInventoryChanged += CheckCollectSteps;
    }

    private void OnDestroy()
    {
        if (playerInventory != null) playerInventory.OnInventoryChanged -= CheckCollectSteps;
    }

    public void AddQuest(QuestData quest)
    {
        if (quest == null || activeQuests.ContainsKey(quest.questID)) return;

        var stepsCopy = new List<QuestStep>();
        foreach (var step in quest.steps)
        {
            stepsCopy.Add(new QuestStep
            {
                stepID = step.stepID,
                description = step.description,
                type = step.type,
                requiredItem = step.requiredItem,
                requiredAmount = step.requiredAmount,
                targetObjectID = step.targetObjectID,
                targetZoneID = step.targetZoneID,
                isComplete = false
            });
        }

        activeQuests[quest.questID] = quest;
        questSteps[quest.questID] = stepsCopy;

        OnQuestAdded?.Invoke(quest);
        Debug.Log($"[QuestManager] Quest added: {quest.questTitle}");
        CheckCollectSteps();
    }

    public bool HasQuest(string questID) => activeQuests.ContainsKey(questID);
    public List<QuestData> GetActiveQuests() => new List<QuestData>(activeQuests.Values);

    public List<QuestStep> GetSteps(string questID)
        => questSteps.ContainsKey(questID) ? questSteps[questID] : new List<QuestStep>();

    public bool HasNotebook()
    {
        Debug.Log($"notebookItem: {notebookItem}, playerInventory: {playerInventory}");
        if (notebookItem == null || playerInventory == null) return false;
        Debug.Log($"CountOf notebook: {playerInventory.CountOf(notebookItem)}");
        return playerInventory.CountOf(notebookItem) > 0;
    }

    private void CheckCollectSteps()
    {
        if (playerInventory == null) return;
        foreach (var kvp in questSteps)
            foreach (var step in kvp.Value)
            {
                if (step.isComplete || step.type != QuestStepType.Collect || step.requiredItem == null) continue;
                if (playerInventory.CountOf(step.requiredItem) >= step.requiredAmount)
                    CompleteStep(kvp.Key, step.stepID);
            }
    }

    public void NotifyObjectInteracted(string objectID)
    {
        foreach (var kvp in questSteps)
            foreach (var step in kvp.Value)
            {
                if (step.isComplete) continue;
                if ((step.type == QuestStepType.Repair || step.type == QuestStepType.Interact)
                    && step.targetObjectID == objectID)
                    CompleteStep(kvp.Key, step.stepID);
            }
    }

    private void CompleteStep(string questID, string stepID)
    {
        if (!questSteps.ContainsKey(questID)) return;
        QuestStep target = null;
        foreach (var step in questSteps[questID])
            if (step.stepID == stepID) { target = step; break; }

        if (target == null || target.isComplete) return;
        target.isComplete = true;
        OnStepCompleted?.Invoke(activeQuests[questID], target);
        Debug.Log($"[QuestManager] Step complete: {target.description}");

        bool allDone = true;
        foreach (var step in questSteps[questID])
            if (!step.isComplete) { allDone = false; break; }

        if (allDone)
        {
            OnQuestCompleted?.Invoke(activeQuests[questID]);
            Debug.Log($"[QuestManager] Quest complete: {activeQuests[questID].questTitle}");
        }
    }
}