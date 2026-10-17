using UnityEngine;

public class NailZone : MonoBehaviour
{
    public SpriteRenderer highlightSprite;
    public Color normalColor = new Color(0, 1, 0, 0.3f);
    public Color highlightColor = new Color(0, 1, 0, 0.7f);

    [HideInInspector] public bool isOccupied = false;
    [HideInInspector] public bool isHammered = false;
    [HideInInspector] public NailDragger placedNail = null;

    public void HideHighlight()
    {
        if (highlightSprite != null)
            highlightSprite.color = new Color(0, 1, 0, 0);
    }

    public void SetHighlight(bool active)
    {
        if (highlightSprite != null)
            highlightSprite.color = active ? highlightColor : new Color(0, 1, 0, 0);
    }

    public bool TryPlace(NailDragger nail)
    {
        if (isOccupied) return false;
        isOccupied = true;
        placedNail = nail;
        HideHighlight();
        nail.transform.position = transform.position;
        nail.isPlaced = true;
        nail.transform.SetParent(null);
        nail.enabled = false;

        SpriteRenderer sr = nail.GetComponent<SpriteRenderer>();
        if (sr != null && nail.dragSprite != null)
            sr.sprite = nail.dragSprite;

        return true;
    }

    public void Hammer()
    {
        isHammered = true;
        if (placedNail != null)
            placedNail.gameObject.SetActive(false);
    }
}