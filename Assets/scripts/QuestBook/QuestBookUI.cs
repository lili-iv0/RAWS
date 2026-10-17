using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestBookUI : MonoBehaviour
{
    [Header("Layout")]
    public Transform leftPageContainer;
    public Transform rightPageContainer;
    public GameObject stepEntryPrefab;
    public int maxEntriesPerPage = 8;

    [Header("No Notebook Message")]//unused
    public GameObject noNotebookPopupPrefab;
    public float popupDuration = 2f;

    public Button closeButton;
    private QuestManager questManager;
    private bool isOpen = false;

    private void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
    }

    public void SetQuestManager(QuestManager qm)
    {
        questManager = qm;
        questManager.OnQuestAdded += _ => { if (isOpen) Refresh(); };
        questManager.OnStepCompleted += (q, s) => { if (isOpen) Refresh(); };
        questManager.OnQuestCompleted += _ => { if (isOpen) Refresh(); };
    }

    public void Toggle()
    {
        if (isOpen) Close(); else Open();
    }

    private void Open()
    {
        if (questManager == null) return;

        bool hasNotebook = questManager.HasNotebook();

        if (hasNotebook)
        {
            isOpen = true;
            gameObject.SetActive(true);
            if (leftPageContainer != null) leftPageContainer.gameObject.SetActive(true);
            if (rightPageContainer != null) rightPageContainer.gameObject.SetActive(true);
            Refresh();
        }
        else
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null && noNotebookPopupPrefab != null)
            {
                var popup = Instantiate(noNotebookPopupPrefab,
                    player.transform.position + Vector3.up * 1.5f,
                    Quaternion.identity);
                Destroy(popup, popupDuration);
            }
        }
    }

    private void Close()
    {
        isOpen = false;
        gameObject.SetActive(false);
    }

    private void Refresh()
    {
        foreach (Transform child in leftPageContainer) Destroy(child.gameObject);
        foreach (Transform child in rightPageContainer) Destroy(child.gameObject);

        int count = 0;

        foreach (var quest in questManager.GetActiveQuests())
        {
            foreach (var step in questManager.GetSteps(quest.questID))
            {
                if (stepEntryPrefab == null) continue;

                Transform container = count < maxEntriesPerPage ? leftPageContainer : rightPageContainer;
                var stepGO = Instantiate(stepEntryPrefab, container);
                var entry = stepGO.GetComponent<StepEntryUI>();
                Debug.Log($"entry null: {entry == null}");

                if (entry != null)
                {
                    entry.Setup(step.description, step.isComplete);
                }

                count++;
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(leftPageContainer.GetComponent<RectTransform>());
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rightPageContainer.GetComponent<RectTransform>());
            }
        }
    }
}