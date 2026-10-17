using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class HammerMechanic : MonoBehaviour
{
    private Animator animator;
    private bool isSwinging = false;

    [Header("Movement Bounds")]
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -5f;
    public float maxY = 3f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hammerSound;
    public float hammerSoundDelay = -2.9f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        FollowCursor();

        if (!isSwinging && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mouseScreen = Mouse.current.position.ReadValue();
            Vector3 worldPos = InputHelper.ScreenToWorld(mouseScreen);

            int hotbarLayer = LayerMask.GetMask("Hotbar");
            int allExceptHotbar = ~hotbarLayer;

            Collider2D[] hits = Physics2D.OverlapPointAll(worldPos, allExceptHotbar);
            foreach (Collider2D hit in hits)
            {
                NailDragger nail = hit.GetComponent<NailDragger>();
                if (nail != null && nail.isPlaced)
                {
                    isSwinging = true;
                    animator.SetTrigger("Swing");
                    StartCoroutine(PlaySoundDelayed(hammerSoundDelay));
                    NailManager.Instance.HammerNail(nail);
                    break;
                }

                NailZone zone = hit.GetComponent<NailZone>();
                if (zone != null && zone.placedNail != null && !zone.isHammered)
                {
                    isSwinging = true;
                    animator.SetTrigger("Swing");
                    StartCoroutine(PlaySoundDelayed(hammerSoundDelay));
                    NailManager.Instance.HammerNail(zone.placedNail);
                    break;
                }
            }
        }
    }

    IEnumerator PlaySoundDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (audioSource != null && hammerSound != null)
            audioSource.PlayOneShot(hammerSound);
    }

    void FollowCursor()
    {
        Vector3 worldPos = Vector3.zero;
        bool hasInput = false;

        if (Mouse.current != null)
        {
            Vector2 mouseScreen = Mouse.current.position.ReadValue();
            worldPos = InputHelper.ScreenToWorld(mouseScreen);
            hasInput = true;
        }
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchScreen = Touchscreen.current.primaryTouch.position.ReadValue();
            worldPos = InputHelper.ScreenToWorld(touchScreen);
            hasInput = true;
        }

        if (hasInput)
        {
            worldPos.x = Mathf.Clamp(worldPos.x, minX, maxX);
            worldPos.y = Mathf.Clamp(worldPos.y, minY, maxY);
            worldPos.z = 0;
            transform.position = worldPos;
        }
    }

    public void OnSwingComplete()
    {
        isSwinging = false;
    }
}