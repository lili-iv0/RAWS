using UnityEngine;

public class OSTManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource daySource;
    public AudioSource indoorSource;
    public AudioSource nightSource;
    public AudioSource postStormSource;

    [Header("OST Clips")]
    public AudioClip dayClip;
    public AudioClip indoorClip;
    public AudioClip nightClip;
    public AudioClip postStormClip;

    [Header("References")]
    public GameClock clock;

    [Header("Transition")]
    [Tooltip("How many seconds to fade between tracks.")]
    public float fadeDuration = 2f;

    private AudioSource _currentSource;
    private bool _postStormUnlocked = false;

    private enum MusicState { Day, Indoor, Night, PostStorm }
    private MusicState _currentState = (MusicState)(-1);


    private void Start()
    {
        SetupSource(daySource,       dayClip);
        SetupSource(indoorSource,    indoorClip);
        SetupSource(nightSource,     nightClip);
        SetupSource(postStormSource, postStormClip);

        if (clock != null)
        {
            clock.OnDayStart   += OnDayStart;
            clock.OnNightStart += OnNightStart;
        }
        else
        {
            Debug.LogWarning("[OSTManager] No GameClock assigned.");
        }

  
        EvaluateState();
    }

    private void OnDestroy()
    {
        if (clock == null) return;
        clock.OnDayStart   -= OnDayStart;
        clock.OnNightStart -= OnNightStart;
    }

    private void Update()
    {
       
        EvaluateState();
    }
    public void MuteForStorm()
    {
        StopAllCoroutines();
        if (_currentSource != null)
            StartCoroutine(FadeOut(_currentSource));
    }

    private System.Collections.IEnumerator FadeOut(AudioSource source)
    {
        float elapsed = 0f;
        float startVolume = source.volume;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
            yield return null;
        }

        source.volume = 0f;
        source.Pause();
    }

    public void UnlockPostStormOST()
    {
        _postStormUnlocked = true;
        Debug.Log("[OSTManager] Post-storm OST unlocked.");
        EvaluateState();
    }


    private void OnDayStart()   => EvaluateState();
    private void OnNightStart() => EvaluateState();


    private void EvaluateState()
    {
        MusicState target = ResolveState();
        if (target == _currentState) return;

        _currentState = target;
        AudioSource next = StateToSource(target);
        TransitionTo(next);
    }

    private MusicState ResolveState()
    {
        // Night overrides everything.
        if (clock != null && clock.isNight)
            return MusicState.Night;

        // Post-storm overrides day and indoor (but not night).
        if (_postStormUnlocked)
            return MusicState.PostStorm;

        // Indoor overrides day only.
        if (Zone.PlayerIsIndoor)
            return MusicState.Indoor;

        return MusicState.Day;
    }

    private AudioSource StateToSource(MusicState state)
    {
        return state switch
        {
            MusicState.Day       => daySource,
            MusicState.Indoor    => indoorSource,
            MusicState.Night     => nightSource,
            MusicState.PostStorm => postStormSource,
            _                    => daySource
        };
    }

    private void TransitionTo(AudioSource next)
    {
        if (next == null) return;

        StopAllCoroutines();
        StartCoroutine(CrossFade(_currentSource, next));
        _currentSource = next;
    }

    

    private System.Collections.IEnumerator CrossFade(AudioSource outSource, AudioSource inSource)
    {
  
        if (inSource != null && !inSource.isPlaying)
        {
            inSource.volume = 0f;
            inSource.Play();
        }

        float elapsed = 0f;
        float outStart = outSource != null ? outSource.volume : 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            if (outSource != null) outSource.volume = Mathf.Lerp(outStart, 0f, t);
            if (inSource  != null) inSource.volume  = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        if (outSource != null) { outSource.volume = 0f; outSource.Pause(); }
        if (inSource  != null)   inSource.volume  = 1f;
    }

    public void ResumeAfterStorm()
    {
        _currentSource = null;
        _currentState = (MusicState)(-1);
        EvaluateState();
    }

    private void SetupSource(AudioSource source, AudioClip clip)
    {
        if (source == null) return;
        source.clip   = clip;
        source.loop   = true;
        source.volume = 0f;
        source.playOnAwake = false;
    }
}
