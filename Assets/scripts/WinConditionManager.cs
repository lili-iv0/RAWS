using System.Collections;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;


//need muna maka repair ng ilang windows bago mag start yung timer, tapos pag nag start na yung timer, kailangan mag survive ng set in-game hours.
public class WinConditionManager : MonoBehaviour
{
    [Header("Window Quest Data")]
    [Tooltip("All 7 window IDs sourced from your quest data.")]
    public string[] windowIDs = new string[7];

    [Tooltip("Minimum windows that must be repaired to satisfy the win condition.")]
    public int windowsRequiredToWin = 4;

    [Header("Survival Timer")]
    [Tooltip("How many in-game hours the player must survive after the storm ends.")]
    public float inGameHoursToSurvive = 8f;

    [Header("Win Screen")]
    public GameObject winScreenPrefab;
    public float fadeInDuration = 1f;

    public GameClock clock;
    private Coroutine _survivalTimerCoroutine;

    public enum ConditionState { Pending, InProgress, Won, Lost }
    public ConditionState State { get; private set; } = ConditionState.Pending;

    public bool StormHasEnded { get; private set; } = false;

    //In-game hours elapsed since the storm ended 
    [Header("Debug Info")]
    [SerializeField] private float _hoursSurvivedAfterStorm;
    public float HoursSurvivedAfterStorm => _hoursSurvivedAfterStorm;
    [SerializeField] private int _fixedWindowCountDisplay;
    [SerializeField] private ConditionState _stateDisplay;

    //Number of windows currently fixed (polled each frame)
    public int FixedWindowCount { get; private set; } = 0;



    public void NotifyStormEnded()
    {
        if (StormHasEnded) return;

        StormHasEnded = true;
        State = ConditionState.InProgress;

        Debug.Log("[WinCondition] Storm has ended. Survival countdown will start " +
                  "once window requirement is met.");


        StartCoroutine(EvaluationLoop());
    }


    public void NotifyPlayerDied()
    {
        if (State == ConditionState.Won || State == ConditionState.Lost) return;

        TriggerLose("Player died.");
    }

    private GameObject _activeWinScreen;



    private IEnumerator EvaluationLoop()
    {
        bool timerStarted = false;

        while (State == ConditionState.InProgress)
        {

            FixedWindowCount = CountFixedWindows();
            _fixedWindowCountDisplay = FixedWindowCount;
            _stateDisplay = State;

            bool windowsMet = FixedWindowCount >= windowsRequiredToWin;

            // Start the survival timer the first time both conditions align:
            //   � storm has ended (always true here)
            //   � enough windows are fixed
            if (windowsMet && !timerStarted)
            {
                timerStarted = true;
                Debug.Log($"[WinCondition] {FixedWindowCount}/{windowIDs.Length} windows fixed. " +
                           "Starting 8-hour survival timer.");
                _survivalTimerCoroutine = StartCoroutine(SurvivalTimer());
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator SurvivalTimer()
    {
        _hoursSurvivedAfterStorm = 0f;
        float totalRealSeconds = inGameHoursToSurvive * clock.realSecondsPerGameMinute * GameClock.MinutesPerHour;
        float elapsed = 0f;

        while (elapsed < totalRealSeconds)
        {
            // Bail out early if state changed externally (e.g. player died).
            if (State != ConditionState.InProgress) yield break;

            elapsed += Time.deltaTime;
            _hoursSurvivedAfterStorm = elapsed / (clock.realSecondsPerGameMinute * GameClock.MinutesPerHour);

            yield return null; // tick every frame for accurate HoursSurvived reporting
        }


        FixedWindowCount = CountFixedWindows();

        if (FixedWindowCount >= windowsRequiredToWin)
        {
            TriggerWin();
        }
        else
        {

            TriggerLose($"Survived the time but only {FixedWindowCount}/" +
                        $"{windowsRequiredToWin} windows were repaired.");
        }
    }


    private int CountFixedWindows()
    {
        if (GameData.Instance == null)
        {
            Debug.LogWarning("[WinCondition] GameData.Instance is null � cannot check windows.");
            return 0;
        }

        int count = 0;
        foreach (string id in windowIDs)
        {
            if (GameData.Instance.IsWindowFixed(id))
                count++;
        }
        return count;
    }

    private void TriggerWin()
    {
        if (State == ConditionState.Won || State == ConditionState.Lost) return;

        State = ConditionState.Won;
        StopAllTrackedCoroutines();

        Debug.Log($"[WinCondition] WIN � {FixedWindowCount} windows repaired, " +
                  $"survived {_hoursSurvivedAfterStorm:F1} in-game hours after storm.");

        StartCoroutine(FadeInWinScreen());
    }

    private IEnumerator FadeInWinScreen()
    {
        if (winScreenPrefab == null)
        {
            Debug.LogWarning("[WinCondition] No win screen prefab assigned.");
            yield break;
        }

        _activeWinScreen = Instantiate(winScreenPrefab);
        GameObject winScreen = _activeWinScreen;

        CanvasGroup canvasGroup = winScreen.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = winScreen.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        Button tryAgainButton = FindTryAgainButton(winScreen);
        if (tryAgainButton != null)
        {
            tryAgainButton.interactable = false;
            tryAgainButton.onClick.AddListener(() => StartCoroutine(FadeOutAndRestart()));
            tryAgainButton.interactable = true;
        }
    }
    private IEnumerator FadeOutAndRestart()
    {
        if (_activeWinScreen != null)
        {
            CanvasGroup canvasGroup = _activeWinScreen.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = _activeWinScreen.AddComponent<CanvasGroup>();

            Button tryAgainButton = FindTryAgainButton(_activeWinScreen);
            if (tryAgainButton != null)
                tryAgainButton.interactable = false;

            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeInDuration);
                yield return null;
            }

            canvasGroup.alpha = 0f;
        }

        Destroy(_activeWinScreen);
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        Debug.Log("[WinCondition] Try Again - Reloading Scene");
    }

    private Button FindTryAgainButton(GameObject screen)
    {
        Transform t = screen.transform.Find("TryAgainButton");
        if (t != null) return t.GetComponent<Button>();

        t = screen.transform.Find("RestartButton");
        if (t != null) return t.GetComponent<Button>();

        Button[] buttons = screen.GetComponentsInChildren<Button>();
        foreach (Button btn in buttons)
        {
            string n = btn.name.ToLower();
            if (n.Contains("tryagain") || n.Contains("restart"))
                return btn;
        }

        return null;
    }

    private void TriggerLose(string reason)
    {
        if (State == ConditionState.Won || State == ConditionState.Lost) return;
        State = ConditionState.Lost;
        StopAllTrackedCoroutines();
        Debug.Log($"[WinCondition] LOSE � {reason}");
    }

    private void StopAllTrackedCoroutines()
    {
        if (_survivalTimerCoroutine != null)
        {
            StopCoroutine(_survivalTimerCoroutine);
            _survivalTimerCoroutine = null;
        }
        StopCoroutine(nameof(EvaluationLoop)); // safe even if not running
    }


}