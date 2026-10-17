using UnityEngine;

public class PlankDragger : MonoBehaviour
{
    public PlacementZone targetZone;
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 originalPosition;
    private Collider2D myCollider;
    public bool isPlaced = false;

    public static PlankDragger currentlyDragging = null;

    void Start()
    {
        originalPosition = transform.position;
        myCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
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
            }
        }

        if (InputHelper.GetInputHeld(out screenPos) && isDragging)
        {
            Vector3 worldPos = InputHelper.ScreenToWorld(screenPos);
            transform.position = worldPos + offset;
            if (targetZone != null)
                PlankManager.Instance.OnPlankDragging(worldPos, targetZone);
        }

        if (InputHelper.GetInputUp(out screenPos) && isDragging)
        {
            isDragging = false;
            currentlyDragging = null;
            if (targetZone != null)
                PlankManager.Instance.OnPlankReleased(this, originalPosition, targetZone);
        }
    }
}