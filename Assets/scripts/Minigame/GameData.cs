using UnityEngine;
using System.Collections.Generic;

public class GameData : MonoBehaviour
{
    public static GameData Instance;
    public float playerHealth = 100f;
    public float playerHunger = 100f;
    public float playerThirst = 100f;
    public Vector3 playerPosition;
    public bool hasPosition = false;
    public string currentWindowID = "";
    public List<string> fixedWindows = new List<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsWindowFixed(string windowID)
    {
        return fixedWindows.Contains(windowID);
    }

    public void FixWindow(string windowID)
    {
        if (!fixedWindows.Contains(windowID))
            fixedWindows.Add(windowID);
    }
}