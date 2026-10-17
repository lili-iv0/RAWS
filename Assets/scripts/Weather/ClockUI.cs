using UnityEngine;
using TMPro;

// Drop onto any UI panel — auto-finds GameClock and searches children for TMP labels.
// Name your TMP children "DayLabel" and "TimeLabel" for auto-detection,
// or assign them manually in the Inspector.
public class ClockUI : MonoBehaviour
{
    [Header("UI Labels (auto-found if left empty)")]
    public TextMeshProUGUI dayLabel;
    public TextMeshProUGUI timeLabel;

    private GameClock clock;

    private void Start()
    {
        clock = GameClock.Instance ?? FindAnyObjectByType<GameClock>();

        if (clock == null)
        {
            Debug.LogWarning("[ClockUI] No GameClock found in scene.");
            return;
        }

      

        if (dayLabel  == null) Debug.LogWarning("[ClockUI] DayLabel not found. Assign manually or name child TMP 'DayLabel'.");
        if (timeLabel == null) Debug.LogWarning("[ClockUI] TimeLabel not found. Assign manually or name child TMP 'TimeLabel'.");

        clock.OnTimeChanged += OnTimeChanged;
        Refresh();
    }

    private void OnDestroy()
    {
        if (clock != null)
            clock.OnTimeChanged -= OnTimeChanged;
    }

    private void OnTimeChanged(int day, int hour, int minute) => Refresh();

    private void Refresh()
    {
        if (clock == null) return;
        if (dayLabel  != null) dayLabel.text  = clock.GetFormattedDay();
        if (timeLabel != null) timeLabel.text = clock.GetFormattedTime();
    }

    private TextMeshProUGUI FindLabelByName(string labelName)
    {
        foreach (var tmp in GetComponentsInChildren<TextMeshProUGUI>())
            if (tmp.gameObject.name == labelName)
                return tmp;
        return null;
    }
}
