using UnityEngine;

public class NailDragger : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 originalPosition;
    private Collider2D myCollider;
    private SpriteRenderer spriteRenderer;
    public bool isPlaced = false;

    public Sprite defaultSprite;
    public Sprite dragSprite;

    public static NailDragger currentlyDragging = null;

    void Start()
    {
        originalPosition = transform.position;
        myCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isPlaced) return;

        Vector2 screenPos;

        if (InputHelper.GetInputDown(out screenPos))
        {
            if (currentlyDragging != null) return;
            Vector3 worldPos = InputHelper.ScreenToWorld(screenPos);
            if (myCollider != null && myCollider.OverlapPoint(worldPos))
            {
                isDragging = true;
                currentlyDragging = this;
                offset = transform.position - worldPos;
                if (spriteRenderer != null && dragSprite != null)
                    spriteRenderer.sprite = dragSprite;
            }
        }

        if (InputHelper.GetInputHeld(out screenPos) && isDragging)
        {
            Vector3 worldPos = InputHelper.ScreenToWorld(screenPos);
            transform.position = worldPos + offset;
            NailManager.Instance.OnNailDragging(worldPos);
        }

        if (InputHelper.GetInputUp(out screenPos) && isDragging)
        {
            isDragging = false;
            currentlyDragging = null;
            if (spriteRenderer != null && defaultSprite != null)
                spriteRenderer.sprite = defaultSprite;
            NailManager.Instance.OnNailReleased(this, originalPosition);
        }
    }
}