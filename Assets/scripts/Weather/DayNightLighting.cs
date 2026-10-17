using UnityEngine;
using UnityEngine.Rendering.Universal;

// Drop onto your Global Light 2D GameObject — auto-finds GameClock and StormSystem.
[RequireComponent(typeof(Light2D))]
public class DayNightLighting : MonoBehaviour
{
    [Header("Day Settings")]
    public Color dayColor;
    public float dayIntensity = 1f;

    [Header("Night Settings")]
    public Color nightColor;
    public float nightIntensity = 0.15f;

    [Header("Transition")]
    [Tooltip("In-game hours the transition takes around sunrise/sunset")]
    public float transitionHours = 1f;
    public const int DayStartHour = 5; 
    public const int NightStartHour = 17;  

    [Header("Storm Override")]
    [Tooltip("Color tint applied to lighting during a storm (blends over day/night color)")]
    public Color stormColor = new Color(0.45f, 0.5f, 0.6f, 1f);
    [Tooltip("How much the storm tint blends in at full intensity (0 = no effect, 1 = full override)")]
    [Range(0f, 1f)]
    public float stormColorStrength = 0.5f;
    [Tooltip("Intensity multiplier applied on top of day/night intensity during storm")]
    [Range(0f, 1f)]
    public float stormIntensityMult = 0.6f;
    [Tooltip("Seconds to blend lighting in/out when storm starts or ends")]
    public float stormBlendSpeed = 2f;

   
    private Light2D globalLight;
    private GameClock clock;

    private float stormBlend = 1f;   // 0 = clear, 1 = full storm
    private bool isStorming = false;

    private float sunriseStart => GameClock.DayStartHour - transitionHours;
    private float sunriseEnd => GameClock.DayStartHour + transitionHours;
    private float sunsetStart => GameClock.NightStartHour - transitionHours;
    private float sunsetEnd => GameClock.NightStartHour + transitionHours;

    // Called by StormSystem
    public void SetStorming(bool storming)
    {
        isStorming = storming;
    }

    //Lifecycle 
    private void Awake()
    {
        globalLight = GetComponent<Light2D>();
    }

    private void Start()
    {
        clock = GameClock.Instance ?? FindAnyObjectByType<GameClock>();
        if (clock == null)
            Debug.LogWarning("[DayNightLighting] No GameClock found in scene.");

        clock.OnTimeChanged += OnTimeChanged;
        UpdateLighting();
    }

    private void OnDestroy()
    {
        if (clock != null) clock.OnTimeChanged -= OnTimeChanged;
    }

    private void Update()
    {
        // Smoothly blend storm lighting in or out
        float target = isStorming ? 1f : 0f;
        if (!Mathf.Approximately(stormBlend, target))
        {
            stormBlend = Mathf.MoveTowards(stormBlend, target, Time.deltaTime / stormBlendSpeed);
            ApplyLighting(GetDayNightBlend(clock.CurrentHour + clock.CurrentMinute / 60f));
        }
    }

    //Clock listener
    private void OnTimeChanged(int day, int hour, int minute)
    {
        UpdateLighting();
    }

    //Lighting
    private void UpdateLighting()
    {
        if (clock == null) return;
        float hourFloat = clock.CurrentHour + (clock.CurrentMinute / 60f);
        ApplyLighting(GetDayNightBlend(hourFloat));
    }

    private void ApplyLighting(float dayNightT)
    {
        // Base day/night values
        Color baseColor = Color.Lerp(nightColor, dayColor, dayNightT);
        float baseIntensity = Mathf.Lerp(nightIntensity, dayIntensity, dayNightT);

        // Blend in storm tint and intensity reduction
        Color finalColor = Color.Lerp(baseColor, stormColor, stormBlend * stormColorStrength);
        float finalIntensity = Mathf.Lerp(baseIntensity, baseIntensity * stormIntensityMult, stormBlend);

        globalLight.color = finalColor;
        globalLight.intensity = finalIntensity;
    }

    private float GetDayNightBlend(float hour)
    {
        if (hour >= sunriseStart && hour <= sunriseEnd) return Mathf.InverseLerp(sunriseStart, sunriseEnd, hour);
        if (hour > sunriseEnd && hour < sunsetStart) return 1f;
        if (hour >= sunsetStart && hour <= sunsetEnd) return 1f - Mathf.InverseLerp(sunsetStart, sunsetEnd, hour);
        return 0f;
    }
}