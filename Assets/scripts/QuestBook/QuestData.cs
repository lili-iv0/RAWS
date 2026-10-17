using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/Quest")]
public class QuestData : ScriptableObject
{
    [Header("Identity")]
    public string questID;
    public string questTitle;
    [TextArea] public string description;

    [Header("Steps")]
    public List<QuestStep> steps = new List<QuestStep>();
}

[System.Serializable]
public class QuestStep
{
    public string stepID;
    public string description;
    public QuestStepType type;

    [Header("Collect")]
    public ItemData requiredItem;
    public int requiredAmount;

    [Header("Repair / Interact")]
    public string targetObjectID;

    [Header("Visit (unused template)")]
    public string targetZoneID;

    [HideInInspector] public bool isComplete = false;
}

public enum QuestStepType { Collect, Repair, Interact, Visit }
