using UnityEngine;

public class PlankManager : MonoBehaviour
{
    public static PlankManager Instance;
    public PlacementZone[] allZones;
    public GameObject plank1Zone;
    public GameObject plank2Zone;
    public ParticleSystem[] rainInsideAll;

    private bool plank2Unlocked = false;

    void Awake()
    {
        Instance = this;
        HidePlankZones();
    }

    public void ShowPlankZones()
    {
        if (plank1Zone != null) plank1Zone.SetActive(true);
        if (plank2Unlocked && plank2Zone != null) plank2Zone.SetActive(true);
    }

    public void HidePlankZones()
    {
        if (plank1Zone != null) plank1Zone.SetActive(false);
        if (plank2Zone != null) plank2Zone.SetActive(false);
    }

    public void UnlockPlank2()
    {
        plank2Unlocked = true;
        if (plank2Zone != null) plank2Zone.SetActive(true);
    }

    public void CheckAllPlaced()
    {
        foreach (var zone in allZones)
        {
            if (!zone.isOccupied) return;
        }
        foreach (var rain in rainInsideAll)
        {
            if (rain != null) rain.Stop(true);
        }
    }

    public void OnPlankDragging(Vector3 worldPos, PlacementZone targetZone)
    {
        if (targetZone == null) return;
        if (!targetZone.isOccupied)
        {
            Collider2D col = targetZone.GetComponent<Collider2D>();
            if (col == null) return;
            bool isNear = col.OverlapPoint(worldPos);
            targetZone.SetHighlight(isNear);
        }
    }

    public void OnPlankReleased(PlankDragger plank, Vector3 originalPosition, PlacementZone targetZone)
    {
        if (targetZone == null)
        {
            plank.transform.position = originalPosition;
            return;
        }

        Collider2D col = targetZone.GetComponent<Collider2D>();
        if (col != null && col.OverlapPoint(plank.transform.position))
        {
            if (targetZone.TryPlace(plank))
            {
                CheckAllPlaced();
                return;
            }
        }

        plank.transform.position = originalPosition;
        targetZone.SetHighlight(false);
    }
}