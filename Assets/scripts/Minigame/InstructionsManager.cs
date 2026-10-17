using UnityEngine;

public class InstructionsManager : MonoBehaviour
{
    public GameObject instructionsPanel;

    void Start()
    {
        Time.timeScale = 0f;
        if (instructionsPanel != null)
            instructionsPanel.SetActive(true);
    }

    public void StartGame()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}