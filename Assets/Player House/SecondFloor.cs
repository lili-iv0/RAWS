using UnityEngine;

public class SecondFloor : MonoBehaviour
{

    public GameObject connectingFloor;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
           GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = connectingFloor.transform.position;
        }
    }
}