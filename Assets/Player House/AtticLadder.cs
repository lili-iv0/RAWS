using UnityEngine;

public class AtticLadder : MonoBehaviour, IInteractable
{
    bool canGo;
    public GameObject connectingFloor;

    void Update()
    {
        if (canGo && Input.GetKeyDown(KeyCode.E))
            Teleport();
    }

    public void Interact(Player player)
    {
        var lockable = GetComponent<LockableLadder>();
        if (lockable != null && lockable.isLocked)
        {
            lockable.TryUnlock(player);
            return;
        }

        if (canGo) Teleport();
    }

    private void Teleport()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = connectingFloor.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) canGo = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) canGo = false;
    }
}