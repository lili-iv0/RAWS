using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("Panels")]
    public GameObject instructionsPanel;
    public GameObject gameOverPanel;
    public GameObject winPanel;

    [Header("Timer")]
    public float timeLimit = 40f;
    public TextMeshProUGUI timerText;
    private float currentTime;

    [Header("Win Delay")]
    public float winDelay = 1.5f;

    [Header("Scene")]
    public string mainSceneName = "MainScene";

    [Header("Fade")]
    public UnityEngine.UI.Image fadeImage;
    public float fadeDuration = 1f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip winSound;
    public AudioClip gameOverSound;
    public AudioClip timerWarningSound;
    public float WinSoundDelay = 1f;
    private bool isGameOver = false;
    private bool gameWon = false;
    private bool gameStarted = false;
    private bool timerWarningSounded = false;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Time.timeScale = 0f;
        isGameOver = false;
        gameWon = false;
        gameStarted = false;
        timerWarningSounded = false;
        currentTime = timeLimit;

        if (instructionsPanel != null) instructionsPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);

        if (timerText != null) timerText.text = timeLimit.ToString();

        StartCoroutine(FadeIn());
    }

    void Update()
    {
        if (!gameStarted || isGameOver || gameWon) return;

        currentTime -= Time.deltaTime;
        currentTime = Mathf.Clamp(currentTime, 0f, timeLimit);

        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(currentTime);
            timerText.text = seconds.ToString();
            timerText.color = currentTime <= 10f ? Color.red : Color.white;
        }

        if (currentTime <= 10f && !timerWarningSounded)
        {
            timerWarningSounded = true;
            if (audioSource != null && timerWarningSound != null)
                audioSource.PlayOneShot(timerWarningSound);
        }

        if (currentTime <= 0)
            GameOver();
    }

    public void StartGame()
    {
        gameStarted = true;
        if (instructionsPanel != null) instructionsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void WinGame()
    {
        if (isGameOver || gameWon) return;
        gameWon = true;

        if (GameData.Instance != null)
            GameData.Instance.FixWindow(GameData.Instance.currentWindowID);

        StartCoroutine(WinSequence());
    }

    IEnumerator WinSequence()
    {
        if (audioSource != null && winSound != null)
            audioSource.PlayOneShot(winSound);

        yield return new WaitForSecondsRealtime(winDelay);
        Time.timeScale = 0f;
        if (winPanel != null) winPanel.SetActive(true);
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;

        if (audioSource != null && gameOverSound != null)
            audioSource.PlayOneShot(gameOverSound);

        if (GameData.Instance != null)
            GameData.Instance.playerHealth = 0;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMain()
    {
        Time.timeScale = 1f;
        StartCoroutine(FadeOut(mainSceneName));
    }

    IEnumerator FadeIn()
    {
        if (fadeImage == null) yield break;
        fadeImage.color = new Color(0, 0, 0, 1);
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            fadeImage.color = new Color(0, 0, 0, 1f - (timer / fadeDuration));
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    IEnumerator FadeOut(string sceneName)
    {
        if (fadeImage == null)
        {
            SceneManager.LoadScene(sceneName);
            yield break;
        }
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            fadeImage.color = new Color(0, 0, 0, timer / fadeDuration);
            yield return null;
        }
        SceneManager.LoadScene(sceneName);
    }
}