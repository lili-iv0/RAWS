using UnityEngine;

public class PlacementZone : MonoBehaviour
{
    public SpriteRenderer highlightSprite;
    public Transform snapPoint;
    public Color normalColor = new Color(0, 1, 0, 0.3f);
    public Color highlightColor = new Color(0, 1, 0, 0.7f);

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip placeSound;

    [HideInInspector] public bool isOccupied = false;

    void Start()
    {
        SetHighlight(false);
    }

    public void SetHighlight(bool active)
    {
        if (highlightSprite != null)
            highlightSprite.color = active ? highlightColor : normalColor;
    }

    public bool TryPlace(PlankDragger plank)
    {
        if (isOccupied) return false;
        isOccupied = true;
        SetHighlight(false);
        if (snapPoint != null)
            plank.transform.position = snapPoint.position;
        else
            plank.transform.position = transform.position;

        plank.isPlaced = true;
        plank.transform.SetParent(null);
        plank.gameObject.SetActive(true);
        plank.enabled = false;

        if (audioSource != null && placeSound != null)
            audioSource.PlayOneShot(placeSound);

        if (gameObject.name == "PLANK1ZONE")
            NailManager.Instance.ShowNailZonesForPlank(1);
        else if (gameObject.name == "PLANK2ZONE")
            NailManager.Instance.ShowNailZonesForPlank(2);

        return true;
    }
}