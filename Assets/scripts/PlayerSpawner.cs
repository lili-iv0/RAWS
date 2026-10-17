using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    void Start()
    {
        if (GameData.Instance != null && GameData.Instance.hasPosition)
        {
            transform.position = GameData.Instance.playerPosition;
        }
    }
}