using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRadius = 1.5f;
    public LayerMask interactableLayer;

    [Header("Interact Prompt")]
    public GameObject interactPromptPrefab;
    public string[] excludePromptTags;//deprecated but kept for flexibility

    [Header("Mobile interact button")]
    public GameObject interactButton;

    [Header("SFX")]
    public GameObject DayBGM;

    private IInteractable nearestInteractable;
    private GameObject currentPrompt;

   

    private void Update()
    {
        DetectNearest();
        if (Input.GetKeyDown(KeyCode.E) && nearestInteractable != null)
            nearestInteractable.Interact(this);
    }

    private void DetectNearest()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius, interactableLayer);
        float closestDist = Mathf.Infinity;
        IInteractable closest = null;

        foreach (Collider2D hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable == null) continue;

            bool excluded = false;
            foreach (string tag in excludePromptTags)
                if (hit.CompareTag(tag)) { excluded = true; break; }
            if (excluded) continue;

            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < closestDist) { closestDist = dist; closest = interactable; }
        }

        if (closest != nearestInteractable)
        {
            nearestInteractable = closest;
            if (currentPrompt != null) Destroy(currentPrompt);

            if (nearestInteractable != null && interactPromptPrefab != null)
            {
                var target = ((MonoBehaviour)nearestInteractable).transform;
                currentPrompt = Instantiate(interactPromptPrefab,
                    target.position + new Vector3(0f, 1f, 0f),
                    Quaternion.identity,
                    target);

                if (interactButton != null) interactButton.SetActive(true);
            }
            else
            {
                if (interactButton != null) interactButton.SetActive(false);
            }

            Debug.Log(nearestInteractable != null
                ? $"[Interaction] Nearest: {((MonoBehaviour)nearestInteractable).gameObject.name}"
                : "[Interaction] No interactable in range.");
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (Time.timeScale == 0) GameManager.instance.resume();
            else GameManager.instance.pause();

        }

    }



    public void OnInteractButton()
    {
        if (nearestInteractable != null)
            nearestInteractable.Interact(this);
    }

    public void OnUseItemButton()
    {
        var itemUser = GetComponent<ItemUser>();
        if (itemUser != null) itemUser.UseSelected();
    }
    public void pauseGame()
    {
        if (Time.timeScale == 0) GameManager.instance.resume();
        else GameManager.instance.pause();
    }

}