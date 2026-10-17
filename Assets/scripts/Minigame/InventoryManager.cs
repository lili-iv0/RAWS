using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public GameObject nailItem1;
    public GameObject nailItem2;
    public GameObject nailItem3;
    public GameObject nailItem4;
    public GameObject hammerItem;
    public GameObject plankItem1;
    public GameObject plankItem2;

    public GameObject nailHotbar;
    public GameObject hammerHotbar;
    public GameObject plankHotbar;

    private GameObject currentActive = null;
    private Camera mainCam;

    void Awake()
    {
        Instance = this;
        mainCam = Camera.main;
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            ToggleNails();

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            ToggleItem(hammerItem);

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            TogglePlanks();

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = mainCam.ScreenToWorldPoint(mousePos);
            worldPos.z = 0;

            int hotbarLayer = LayerMask.GetMask("Hotbar");
            Collider2D hit = Physics2D.OverlapPoint(worldPos, hotbarLayer);
            if (hit != null)
            {
                if (nailHotbar != null && hit.gameObject == nailHotbar)
                    ToggleNails();
                else if (hammerHotbar != null && hit.gameObject == hammerHotbar)
                    ToggleItem(hammerItem);
                else if (plankHotbar != null && hit.gameObject == plankHotbar)
                    TogglePlanks();
            }
        }
    }

    public void ShowPlank2Nails()
    {
        NailDragger nd3 = nailItem3.GetComponent<NailDragger>();
        NailDragger nd4 = nailItem4.GetComponent<NailDragger>();
        if (nd3 != null && !nd3.isPlaced) nailItem3.SetActive(true);
        if (nd4 != null && !nd4.isPlaced) nailItem4.SetActive(true);
    }

    void ToggleNails()
    {
        if (currentActive == nailItem1)
        {
            HideAllUnplacedNails();
            currentActive = null;
            NailManager.Instance.HideNailHighlights();
            return;
        }

        if (currentActive != null && currentActive != plankItem1 && currentActive != plankItem2 && currentActive != nailItem1)
            currentActive.SetActive(false);

        HideUnplacedPlanks();
        PlankManager.Instance.HidePlankZones();
        NailManager.Instance.HideNailHighlights();

        ShowAvailableNails();
        currentActive = nailItem1;
        NailManager.Instance.ShowNailHighlights();
    }

    void ShowAvailableNails()
    {
        NailDragger nd1 = nailItem1.GetComponent<NailDragger>();
        NailDragger nd2 = nailItem2.GetComponent<NailDragger>();
        NailDragger nd3 = nailItem3.GetComponent<NailDragger>();
        NailDragger nd4 = nailItem4.GetComponent<NailDragger>();

        if (nd1 != null && !nd1.isPlaced) nailItem1.SetActive(true);
        if (nd2 != null && !nd2.isPlaced) nailItem2.SetActive(true);
        if (nd3 != null && !nd3.isPlaced) nailItem3.SetActive(true);
        if (nd4 != null && !nd4.isPlaced) nailItem4.SetActive(true);
    }

    void HideAllUnplacedNails()
    {
        NailDragger nd1 = nailItem1.GetComponent<NailDragger>();
        NailDragger nd2 = nailItem2.GetComponent<NailDragger>();
        NailDragger nd3 = nailItem3.GetComponent<NailDragger>();
        NailDragger nd4 = nailItem4.GetComponent<NailDragger>();

        if (nd1 != null && !nd1.isPlaced) nailItem1.SetActive(false);
        if (nd2 != null && !nd2.isPlaced) nailItem2.SetActive(false);
        if (nd3 != null && !nd3.isPlaced) nailItem3.SetActive(false);
        if (nd4 != null && !nd4.isPlaced) nailItem4.SetActive(false);
    }

    void HideUnplacedPlanks()
    {
        PlankDragger pd1 = plankItem1.GetComponent<PlankDragger>();
        PlankDragger pd2 = plankItem2.GetComponent<PlankDragger>();
        if (pd1 != null && !pd1.isPlaced) plankItem1.SetActive(false);
        if (pd2 != null && !pd2.isPlaced) plankItem2.SetActive(false);
    }

    void ShowUnplacedPlanks()
    {
        PlankDragger pd1 = plankItem1.GetComponent<PlankDragger>();
        PlankDragger pd2 = plankItem2.GetComponent<PlankDragger>();
        if (pd1 != null && !pd1.isPlaced) plankItem1.SetActive(true);
        if (pd2 != null && !pd2.isPlaced) plankItem2.SetActive(true);
    }

    void ToggleItem(GameObject item)
    {
        if (currentActive == item)
        {
            item.SetActive(false);
            currentActive = null;
            PlankManager.Instance.HidePlankZones();
            NailManager.Instance.HideNailHighlights();
            return;
        }

        if (currentActive != null && currentActive != plankItem1 && currentActive != plankItem2 && currentActive != nailItem1)
            currentActive.SetActive(false);

        HideUnplacedPlanks();
        HideAllUnplacedNails();
        PlankManager.Instance.HidePlankZones();
        NailManager.Instance.HideNailHighlights();

        item.SetActive(true);
        currentActive = item;
    }

    void TogglePlanks()
    {
        if (currentActive == plankItem1)
        {
            HideUnplacedPlanks();
            currentActive = null;
            PlankManager.Instance.HidePlankZones();
            NailManager.Instance.HideNailHighlights();
            return;
        }

        if (currentActive != null && currentActive != plankItem1 && currentActive != plankItem2 && currentActive != nailItem1)
            currentActive.SetActive(false);

        HideUnplacedPlanks();
        HideAllUnplacedNails();
        PlankManager.Instance.HidePlankZones();
        NailManager.Instance.HideNailHighlights();

        ShowUnplacedPlanks();
        currentActive = plankItem1;
        PlankManager.Instance.ShowPlankZones();
    }
}