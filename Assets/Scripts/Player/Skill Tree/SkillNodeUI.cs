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

    private Player player;
    private Image background;

    private void Start()
    {
        player     = FindFirstObjectByType<Player>();
        background = GetComponent<Image>();

        if (node != null && node.skillData != null)
        {
            if (titleText != null)       titleText.text       = node.skillData.skillName;
            if (descriptionText != null) descriptionText.text = node.skillData.description;
            if (costText != null)        costText.text        = "Cost: " + node.skillData.cost;
        }

        if (unlockButton != null)
            unlockButton.onClick.AddListener(TryUnlock);

        Refresh();
    }

    public void TryUnlock()
    {
        if (node != null)
            node.Unlock(player);
    }

    public void Refresh()
    {
        if (node == null || node.skillData == null) return;

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
                ? node.skillData.skillName + " (Unlocked)"
                : node.skillData.skillName;
    }
}
