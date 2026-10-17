using UnityEngine;

public class NailManager : MonoBehaviour
{
    public static NailManager Instance;

    public NailZone[] allNailZones;
    public NailZone plank1ZoneLeft;
    public NailZone plank1ZoneRight;
    public NailZone plank2ZoneLeft;
    public NailZone plank2ZoneRight;
    public SpriteRenderer plank1Renderer;
    public SpriteRenderer plank2Renderer;
    public Sprite plankNailed1;
    public Sprite plankNailed2;
    public GameObject plank2Zone;
    public GameObject plank2Object;

    private bool plank1NailsDone = false;
    private bool plank2NailsDone = false;

    void Awake()
    {
        Instance = this;
    }

    public void ShowNailZonesForPlank(int plankIndex)
    {
        if (plankIndex == 1)
            plank1ZoneLeft.gameObject.SetActive(true);

        if (plankIndex == 2)
            plank2ZoneLeft.gameObject.SetActive(true);
    }

    public void ShowNailHighlights()
    {
        foreach (var zone in allNailZones)
        {
            if (zone == null) continue;
            if (zone.gameObject.activeSelf && !zone.isOccupied)
            {
                SpriteRenderer sr = zone.GetComponentInChildren<SpriteRenderer>(true);
                if (sr != null)
                    sr.color = new Color(0, 1, 0, 0.7f);
            }
        }
    }

    public void HideNailHighlights()
    {
        foreach (var zone in allNailZones)
        {
            if (zone == null) continue;
            SpriteRenderer sr = zone.GetComponentInChildren<SpriteRenderer>(true);
            if (sr != null)
                sr.color = new Color(0, 1, 0, 0);
        }
    }

    public void OnNailDragging(Vector3 worldPos)
    {
        foreach (var zone in allNailZones)
        {
            if (zone == null) continue;
            if (!zone.isOccupied && zone.gameObject.activeSelf)
            {
                Collider2D col = zone.GetComponent<Collider2D>();
                if (col == null) continue;
                SpriteRenderer sr = zone.GetComponentInChildren<SpriteRenderer>(true);
                if (sr != null)
                    sr.color = col.OverlapPoint(worldPos) ? new Color(0, 1, 0, 0.7f) : new Color(0, 1, 0, 0.3f);
            }
        }
    }

    public void OnNailReleased(NailDragger nail, Vector3 originalPosition)
    {
        foreach (var zone in allNailZones)
        {
            if (zone == null) continue;
            if (!zone.isOccupied && zone.gameObject.activeSelf)
            {
                Collider2D col = zone.GetComponent<Collider2D>();
                if (col != null && col.OverlapPoint(nail.transform.position))
                {
                    if (zone.TryPlace(nail)) return;
                }
            }
        }

        nail.transform.position = originalPosition;
        HideNailHighlights();
    }

    public void HammerNail(NailDragger nail)
    {
        if (plank1ZoneLeft.placedNail == nail && !plank1ZoneLeft.isHammered)
        {
            plank1ZoneLeft.Hammer();
            if (plank1Renderer != null) plank1Renderer.sprite = plankNailed1;
            plank1ZoneRight.gameObject.SetActive(true);
            return;
        }

        if (plank1ZoneRight.placedNail == nail && !plank1ZoneRight.isHammered)
        {
            plank1ZoneRight.Hammer();
            if (plank1Renderer != null) plank1Renderer.sprite = plankNailed2;
            if (!plank1NailsDone)
            {
                plank1NailsDone = true;
                if (plank2Object != null) plank2Object.SetActive(true);
                PlankManager.Instance.UnlockPlank2();
                InventoryManager.Instance.ShowPlank2Nails();
            }
            return;
        }

        if (plank2ZoneLeft.placedNail == nail && !plank2ZoneLeft.isHammered)
        {
            plank2ZoneLeft.Hammer();
            if (plank2Renderer != null) plank2Renderer.sprite = plankNailed1;
            plank2ZoneRight.gameObject.SetActive(true);
            return;
        }

        if (plank2ZoneRight.placedNail == nail && !plank2ZoneRight.isHammered)
        {
            plank2ZoneRight.Hammer();
            if (plank2Renderer != null) plank2Renderer.sprite = plankNailed2;
            if (!plank2NailsDone)
            {
                plank2NailsDone = true;
                GameController.Instance.WinGame();
            }
            return;
        }
    }
}