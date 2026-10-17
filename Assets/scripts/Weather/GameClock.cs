using UnityEngine;
using System;


// Controls in-game time progression for the 7-day week.
public class GameClock : MonoBehaviour
{
    [Header("Time Settings")]
    [Tooltip("How many real seconds = 1 in-game minute. Lower = faster.")]
    public float realSecondsPerGameMinute = 50f;

    [Header("Starting Time")]
    public int startDay    = 1;
    public int startHour   = 6;
    public int startMinute = 0;

    [Header("Read Only — Current Time")]
    [SerializeField] private int   currentDay;
    [SerializeField] private int   currentHour;
    [SerializeField] private int   currentMinute;
    [SerializeField] private float minuteTimer;

    public const int TotalDays      = 7;
    public const int HoursPerDay    = 24;
    public const int MinutesPerHour = 60;
    public const int DayStartHour   = 6;
    public const int NightStartHour = 20;

    public event Action<int, int, int> OnTimeChanged;
    public event Action<int>           OnNewDay;
    public event Action                OnDayStart;
    public event Action                OnNightStart;
    public event Action                OnWeekEnd;

    public bool isNight  { get; private set; }
    public bool isPaused { get; private set; }

    public float TimeOfDayNormalized =>
        (currentHour * MinutesPerHour + currentMinute) / (float)(HoursPerDay * MinutesPerHour);

    public int CurrentDay    => currentDay;
    public int CurrentHour   => currentHour;
    public int CurrentMinute => currentMinute;

    public static GameClock Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        currentDay    = startDay;
        currentHour   = startHour;
        currentMinute = startMinute;
        minuteTimer   = 0f;
        isNight       = currentHour >= NightStartHour || currentHour < DayStartHour;
        OnTimeChanged?.Invoke(currentDay, currentHour, currentMinute);
    }

    private void Update()
    {
        if (isPaused) return;
        minuteTimer += Time.deltaTime;
        if (minuteTimer >= realSecondsPerGameMinute)
        {
            minuteTimer -= realSecondsPerGameMinute;
            AdvanceMinute();
        }
    }

    private void AdvanceMinute()
    {
        currentMinute++;
        if (currentMinute >= MinutesPerHour) { currentMinute = 0; AdvanceHour(); }
        OnTimeChanged?.Invoke(currentDay, currentHour, currentMinute);
    }

    private void AdvanceHour()
    {
        currentHour++;
        if (currentHour == DayStartHour)   { isNight = false; OnDayStart?.Invoke();   Debug.Log($"[Clock] Day {currentDay} — Sunrise"); }
        if (currentHour == NightStartHour) { isNight = true;  OnNightStart?.Invoke(); Debug.Log($"[Clock] Day {currentDay} — Nightfall"); }
        if (currentHour >= HoursPerDay)    { currentHour = 0; AdvanceDay(); }
    }

    private void AdvanceDay()
    {
        currentDay++;
        Debug.Log($"[Clock] Day {currentDay} begins.");
        OnNewDay?.Invoke(currentDay);
        if (currentDay > TotalDays) { isPaused = true; OnWeekEnd?.Invoke(); Debug.Log("[Clock] Week ended."); }
    }

    public void Pause()  => isPaused = true;
    public void Resume() => isPaused = false;
    public void SetTimeScale(float realSecondsPerMinute) => realSecondsPerGameMinute = Mathf.Max(0.01f, realSecondsPerMinute);

    public string GetFormattedTime()
    {
        string period = currentHour >= 12 ? "PM" : "AM";
        int hour12 = currentHour % 12;
        if (hour12 == 0) hour12 = 12;
        return $"{hour12}:{currentMinute:D2} {period}";
    }

    public string GetFormattedDay() => $"Day {currentDay} of {TotalDays}";
}
