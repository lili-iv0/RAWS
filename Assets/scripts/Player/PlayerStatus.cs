using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerStatus : MonoBehaviour
{
    public SpriteRenderer sr;
    public Sprite dedSprite;
    [Header("Status")]
    public float maxHealth = 100f;
    public float health = 100f;
    public float maxHunger = 100f;
    public float hunger = 100f;
    public float maxThirst = 100f;
    public float thirst = 100f;
    [Header("Decay (per second)")]
    public float hungerDecay = 2f;
    public float thirstDecay = 3f;
    public float healthDrainWhenStarving = 5f;
    [Header("Movement Effect")]
    public float lowHealthThreshold = 30f;
    public float normalSpeed = 5f;
    public float lowHealthSpeed = 2.5f;
    [Header("UI")]
    public Image healthBar;
    public Image hungerBar;
    public Image thirstBar;
    [Header("Death Screen")]
    public GameObject deathScreenPrefab;
    [Header("Fade Settings")]
    public float fadeInDuration = 1f;
    public float fadeOutDuration = 1f;
    
    private PlayerMovement playerMovement;
    public bool isDead = false;
    private GameObject activeDeathScreen;
    private bool gameFrozen = false;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();

        if (GameData.Instance != null && GameData.Instance.hasPosition)
        {
            health = GameData.Instance.playerHealth;
            hunger = GameData.Instance.playerHunger;
            thirst = GameData.Instance.playerThirst;
        }

        UpdateBars();
    }

    void Update()
    {
        if (isDead || gameFrozen) return;
        
        hunger -= hungerDecay * Time.deltaTime;
        thirst -= thirstDecay * Time.deltaTime;
        hunger = Mathf.Clamp(hunger, 0f, maxHunger);
        thirst = Mathf.Clamp(thirst, 0f, maxThirst);
        health = Mathf.Clamp(health, 0f, maxHealth);
        if (hunger <= 0f || thirst <= 0f)
            health -= healthDrainWhenStarving * Time.deltaTime;

        if (playerMovement != null)
        {
            if (health <= lowHealthThreshold)
                playerMovement.moveSpeed = lowHealthSpeed;
            else
                playerMovement.moveSpeed = normalSpeed;
        }

        if (health <= 0)
            Die();

        if (Input.GetKeyDown(KeyCode.J))
            TakeDamage(10f);
        if (Input.GetKeyDown(KeyCode.K))
            Eat(15f);
        if (Input.GetKeyDown(KeyCode.L))
            Drink(15f);

        UpdateBars();
    }

    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        gameFrozen = true;
        
        if (playerMovement != null)
        {
            playerMovement.moveSpeed = 0;
            playerMovement.enabled = false;
        }
        
        Animator animator = GetComponent<Animator>();
        if (animator != null)
            animator.enabled = false;
            
        sr.sprite = dedSprite;
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
        
        StartCoroutine(FadeInDeathScreen());
        
        Debug.Log("Player Died");
        WinConditionManager winConditionManager = FindAnyObjectByType<WinConditionManager>();
        winConditionManager?.NotifyPlayerDied();
    }
    
    IEnumerator FadeInDeathScreen()
    {
        if (deathScreenPrefab != null)
        {
            activeDeathScreen = Instantiate(deathScreenPrefab);
            
            CanvasGroup canvasGroup = activeDeathScreen.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = activeDeathScreen.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            
            Button tryAgainButton = FindTryAgainButton(activeDeathScreen);
            if (tryAgainButton != null)
            {
                tryAgainButton.interactable = false;
                tryAgainButton.onClick.AddListener(() => StartCoroutine(FadeOutAndRestart()));
            }
            
            Button exitButton = FindExitButton(activeDeathScreen);
            if (exitButton != null)
            {
                exitButton.interactable = false;
                exitButton.onClick.AddListener(ExitGame);
            }
            
            float elapsedTime = 0f;
            while (elapsedTime < fadeInDuration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
                yield return null;
            }
            
            canvasGroup.alpha = 1f;
            
            if (tryAgainButton != null)
                tryAgainButton.interactable = true;
            if (exitButton != null)
                exitButton.interactable = true;
        }
        else
        {
            Debug.LogError("Death Screen Prefab is not assigned in PlayerStatus!");
        }
    }
    
    IEnumerator FadeOutAndRestart()
    {
        if (activeDeathScreen != null)
        {
            CanvasGroup canvasGroup = activeDeathScreen.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = activeDeathScreen.AddComponent<CanvasGroup>();
            
            Button tryAgainButton = FindTryAgainButton(activeDeathScreen);
            Button exitButton = FindExitButton(activeDeathScreen);
            
            if (tryAgainButton != null)
                tryAgainButton.interactable = false;
            if (exitButton != null)
                exitButton.interactable = false;
            
            float elapsedTime = 0f;
            while (elapsedTime < fadeOutDuration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
                yield return null;
            }
            
            canvasGroup.alpha = 0f;
        }
        
        Destroy(activeDeathScreen);
        
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        
        Debug.Log("Try Again - Reloading Scene");
    }
    
    Button FindTryAgainButton(GameObject deathScreen)
    {
        Transform tryAgainTransform = deathScreen.transform.Find("TryAgainButton");
        if (tryAgainTransform != null)
            return tryAgainTransform.GetComponent<Button>();
        
        tryAgainTransform = deathScreen.transform.Find("RestartButton");
        if (tryAgainTransform != null)
            return tryAgainTransform.GetComponent<Button>();
        
        tryAgainTransform = deathScreen.transform.Find("RespawnButton");
        if (tryAgainTransform != null)
            return tryAgainTransform.GetComponent<Button>();
        
        Button[] buttons = deathScreen.GetComponentsInChildren<Button>();
        foreach (Button btn in buttons)
        {
            string name = btn.name.ToLower();
            if (name.Contains("tryagain") || name.Contains("restart") || name.Contains("respawn"))
                return btn;
        }
        
        return null;
    }
    
    Button FindExitButton(GameObject deathScreen)
    {
        Transform exitTransform = deathScreen.transform.Find("ExitButton");
        if (exitTransform != null)
            return exitTransform.GetComponent<Button>();
        
        exitTransform = deathScreen.transform.Find("QuitButton");
        if (exitTransform != null)
            return exitTransform.GetComponent<Button>();
        
        Button[] buttons = deathScreen.GetComponentsInChildren<Button>();
        foreach (Button btn in buttons)
        {
            string name = btn.name.ToLower();
            if (name.Contains("exit") || name.Contains("quit"))
                return btn;
        }
        
        return null;
    }
    
    public void ExitGame()
    {
        Debug.Log("Exiting Game");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void UpdateBars()
    {
        if (healthBar != null)
            healthBar.fillAmount = health / maxHealth;
        if (hungerBar != null)
            hungerBar.fillAmount = hunger / maxHunger;
        if (thirstBar != null)
            thirstBar.fillAmount = thirst / maxThirst;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        health -= amount;
        health = Mathf.Clamp(health, 0f, maxHealth);
        UpdateBars();
    }

    public void Eat(float amount)
    {
        if (isDead) return;
        hunger += amount;
        hunger = Mathf.Clamp(hunger, 0f, maxHunger);
        UpdateBars();
    }

    public void Drink(float amount)
    {
        if (isDead) return;
        thirst += amount;
        thirst = Mathf.Clamp(thirst, 0f, maxThirst);
        UpdateBars();
    }
}