using UnityEngine;

public class NewsTrigger : MonoBehaviour
{
    public GameObject newsPanel;
    public GameObject controlCanvas;
    public GameObject bagButton;
    public GameObject player;

    public AudioSource audioSource;
    public AudioClip newsPanelClip;

    private bool triggered = false;
    private PlayerMovement playerMovement;
    private Rigidbody2D playerRb;
    private Vector2 savedVelocity;
    private OSTManager ostManager;

    void Start()
    {
        ostManager = FindAnyObjectByType<OSTManager>();
        newsPanel.SetActive(false);
        if (controlCanvas != null)
            controlCanvas.SetActive(true);
        if (bagButton != null)
            bagButton.SetActive(true);
        
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            playerRb = player.GetComponent<Rigidbody2D>();
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!triggered && collision.gameObject.CompareTag("Player"))
        {
            triggered = true;
            ostManager?.MuteForStorm();
            if (audioSource != null && newsPanelClip != null)
            {
                audioSource.PlayOneShot(newsPanelClip);
            }
            if (playerRb != null)
            {
                savedVelocity = playerRb.linearVelocity;
                playerRb.linearVelocity = Vector2.zero;
            }
            
            if (playerMovement != null)
                playerMovement.enabled = false;
            
            if (controlCanvas != null)
                controlCanvas.SetActive(false);
            if (bagButton != null)
                bagButton.SetActive(false);
            
            newsPanel.SetActive(true);
            Invoke("HidePanel", 15f);
        }
    }
    
    void HidePanel()
    {
        newsPanel.SetActive(false);
        if (audioSource != null)
            audioSource.Stop();
        ostManager?.ResumeAfterStorm();
        if (playerMovement != null)
        {
            playerMovement.moveX(0f);
            playerMovement.moveY(0f);
            playerMovement.enabled = true;
        }

        if (playerRb != null)
            playerRb.linearVelocity = Vector2.zero;
        if (controlCanvas != null)
            controlCanvas.SetActive(true);
        if (bagButton != null)
            bagButton.SetActive(true);
        EnablePlayerMovement(); 
    }
    
    public void EnablePlayerMovement()
    {
        if (triggered)
        {
            if (playerMovement != null)
                playerMovement.enabled = true;
        
        }
    }
}