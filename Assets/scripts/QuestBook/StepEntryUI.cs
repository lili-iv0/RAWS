using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StepEntryUI : MonoBehaviour
{
    public TextMeshProUGUI descriptionLabel;
    public Toggle completionToggle;

    [Header("Colors")]
    public Color incompleteColor;
    public Color completeColor = new Color(0.3f, 0.9f, 0.3f, 1f); // green

    public Image checkmarkImage;
    public Image backgroundImage;
    public void Setup(string description, bool isComplete)
    {
        descriptionLabel.text = description;
        completionToggle.isOn = isComplete;
        completionToggle.interactable = false;

       if (isComplete)
{
    if (checkmarkImage  != null) checkmarkImage.color  = completeColor;
    if (backgroundImage != null) backgroundImage.color = completeColor;
    descriptionLabel.color = completeColor;
}
else
{
    if (checkmarkImage  != null) checkmarkImage.color  = incompleteColor;
    if (backgroundImage != null) backgroundImage.color = incompleteColor;
    descriptionLabel.color = incompleteColor;
}
        }
    }
