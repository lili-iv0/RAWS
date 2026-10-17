using UnityEngine;
using System.Collections;

public class StormSystem : MonoBehaviour
{
    [Header("Storm Schedule")]
    [Tooltip("Which in-game days a storm occurs (1-7)")]
    public int[] stormDays = { 2, 4, 6 };
    [Tooltip("What hour the storm starts (0-23)")]
    public int stormStartHour = 14;
    [Tooltip("How many in-game hours the storm lasts")]
    public int stormDurationHours = 4;

    [Header("Player Damage (outdoor only)")]
    [Tooltip("Damage dealt to the player per second while outside during storm")]
    public float playerDamagePerSecond = 2f;

    [Header("Storm Quest")]
    public QuestData stormQuest;

    [Header("Particles")]
    public ParticleSystem rainParticles;
    public ParticleSystem debrisParticles;
    public ParticleSystem fogParticles;

    [Header("Audio")]
    public AudioSource stormAudioSource;
    public AudioSource stormSFXSource;
    public AudioClip stormBGSound;
    public AudioClip stormSFXSound;


    private OSTManager ostManager;
    public bool IsStorming { get; private set; } = false;

    private GameClock clock;
    private Rigidbody2D playerRb;
    private PlayerStatus playerStatus;
    private DayNightLighting dayNightLighting;
    private WinConditionManager winConditionManager;
    private int stormEndHour;
    private int activeStormDay = -1;
    private bool stormEndsNextDay = false;
    private bool hasTriggeredToday = false;

    private float playerDamageTimer = 0f;
    private bool lastKnownIndoor = false;

    private void Start()
    {
        clock = GameClock.Instance ?? FindAnyObjectByType<GameClock>();
        ostManager = FindAnyObjectByType<OSTManager>();
        winConditionManager = FindAnyObjectByType<WinConditionManager>();
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
            playerStatus = player.GetComponent<PlayerStatus>();
        }

        dayNightLighting = FindAnyObjectByType<DayNightLighting>();

        if (clock == null) { Debug.LogWarning("[StormSystem] No GameClock found."); return; }
        if (playerRb == null) Debug.LogWarning("[StormSystem] No Rigidbody2D found on Player.");
        if (playerStatus == null) Debug.LogWarning("[StormSystem] No PlayerStatus found on Player.");
        if (dayNightLighting == null) Debug.LogWarning("[StormSystem] No DayNightLighting found — storm lighting disabled.");

        clock.OnTimeChanged += OnTimeChanged;
        clock.OnNewDay += OnNewDay;

        lastKnownIndoor = Zone.PlayerIsIndoor;

        if (rainParticles != null) rainParticles.Stop();
        if (debrisParticles != null) debrisParticles.Stop();
        if (fogParticles != null) fogParticles.Stop();
    }

    private void OnDestroy()
    {
        if (clock == null) return;
        clock.OnTimeChanged -= OnTimeChanged;
        clock.OnNewDay -= OnNewDay;
    }

    private void Update()
    {
        if (Zone.PlayerIsIndoor != lastKnownIndoor)
        {
            lastKnownIndoor = Zone.PlayerIsIndoor;
            if (IsStorming) UpdateParticles();
        }

        if (!IsStorming || Zone.PlayerIsIndoor) return;

        if (playerStatus != null && !playerStatus.isDead)
        {
            playerDamageTimer += Time.deltaTime;
            if (playerDamageTimer >= 1f)
            {
                playerStatus.TakeDamage(playerDamagePerSecond);
                playerDamageTimer = 0f;
            }
        }
    }

    private void OnNewDay(int day)
    {
        hasTriggeredToday = false;
    }

    private void OnTimeChanged(int day, int hour, int minute)
    {
        if (!IsStorming && !hasTriggeredToday && IsStormDay(day) && hour >= stormStartHour)
        {
            StartStorm(day);
            return;
        }

        if (IsStorming && !stormEndsNextDay && day == activeStormDay && hour >= stormEndHour)
        {
            EndStorm();
            return;
        }

        if (IsStorming && stormEndsNextDay && day == activeStormDay + 1 && hour >= stormEndHour)
        {
            EndStorm();
        }
    }

    private void StartStorm(int day)
    {
        IsStorming = true;
        activeStormDay = day;
        hasTriggeredToday = true;
        playerDamageTimer = 0f;

        int rawEnd = stormStartHour + stormDurationHours;
        stormEndHour = rawEnd % GameClock.HoursPerDay;
        stormEndsNextDay = rawEnd >= GameClock.HoursPerDay;

        ostManager?.MuteForStorm(); 
        UpdateParticles();

        if (stormAudioSource != null && stormBGSound != null)
        {
            stormAudioSource.clip = stormBGSound;
            stormAudioSource.loop = true;
            stormAudioSource.Play();
        }
        if (stormSFXSource != null && stormSFXSound != null)
        {
            stormSFXSource.clip = stormSFXSound;
            stormSFXSource.loop = true;
            stormSFXSource.Play();
        }
        if (dayNightLighting != null)
            dayNightLighting.SetStorming(true);

        if (stormQuest != null)
            QuestManager.Instance?.AddQuest(stormQuest);

        Debug.Log($"[Storm] Storm started on Day {day}. Ends at hour {stormEndHour} (next day: {stormEndsNextDay}).");
    }

    private void EndStorm()
    {
        IsStorming = false;
        activeStormDay = -1;
        stormEndsNextDay = false;
        playerDamageTimer = 0f;

        if (rainParticles != null) rainParticles.Stop();
        if (debrisParticles != null) debrisParticles.Stop();
        if (fogParticles != null) fogParticles.Stop();

        if (stormAudioSource != null)
            stormAudioSource.Stop();
        if (stormSFXSource != null)
            stormSFXSource.Stop();

        if (dayNightLighting != null)
            dayNightLighting.SetStorming(false);
        winConditionManager?.NotifyStormEnded();
        Debug.Log("[Storm] Storm ended.");
        ostManager?.UnlockPostStormOST();

    }

    private void UpdateParticles()
    {
        bool show = IsStorming && !Zone.PlayerIsIndoor;

        if (rainParticles != null)
        {
            if (show && !rainParticles.isPlaying) rainParticles.Play();
            if (!show && rainParticles.isPlaying) rainParticles.Stop();
        }

        if (debrisParticles != null)
        {
            if (show && !debrisParticles.isPlaying) debrisParticles.Play();
            if (!show && debrisParticles.isPlaying) debrisParticles.Stop();
        }

        if (fogParticles != null)
        {
            if (show && !fogParticles.isPlaying) fogParticles.Play();
            if (!show && fogParticles.isPlaying) fogParticles.Stop();
        }
        if (stormSFXSource != null)
        {
            if (show && !stormSFXSource.isPlaying) stormSFXSource.Play();
            if (!show && stormSFXSource.isPlaying) stormSFXSource.Pause();
        }
    }

    private bool IsStormDay(int day)
    {
        foreach (int d in stormDays)
            if (d == day) return true;
        return false;
    }
}