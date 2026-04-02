using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillNodeUI : MonoBehaviour
{
    public SkillNode node;
    public Button unlockButton;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI costText;

    private Color lockedColor    = new Color(0.25f, 0.25f, 0.25f);
    private Color availableColor = new Color(0.8f,  0.65f, 0.1f);
    private Color unlockedColor  = new Color(0.15f, 0.6f,  0.15f);

    private Image background;
    private SkillConfirmBox confirmBox;

    void Start()
    {
        background = GetComponent<Image>();
        confirmBox = FindFirstObjectByType<SkillConfirmBox>(FindObjectsInactive.Include);

        if (node != null)
        {
            if (titleText != null)       titleText.text       = node.skillName;
            if (descriptionText != null) descriptionText.text = node.description;
        }

        if (unlockButton != null)
            unlockButton.onClick.AddListener(OnClick);

        Refresh();
    }

    private void OnClick()
    {
        if (node == null || !node.CanUnlock()) return;
        if (confirmBox != null)
            confirmBox.Show(node);
    }

    public void Refresh()
    {
        if (node == null) return;

        bool unlocked  = node.IsUnlocked();
        bool canUnlock = node.CanUnlock();

        if (unlockButton != null)
            unlockButton.interactable = !unlocked && canUnlock;

        if (background != null)
        {
            if (unlocked)       background.color = unlockedColor;
            else if (canUnlock) background.color = availableColor;
            else                background.color = lockedColor;
        }

        if (titleText != null)
            titleText.text = unlocked
                ? node.skillName + " (Unlocked)"
                : node.skillName;
    }
}