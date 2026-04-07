using UnityEngine;
using TMPro;

/// <summary>
/// Attach to a UI Panel (Canvas child) in the MainWorld scene.
/// DockBoat calls Show() / Hide() when the player enters or exits the boat trigger.
///
/// Scene setup:
///   1. Create a Canvas (Screen Space – Overlay) in MainWorld (or reuse the HUD canvas).
///   2. Add a child Panel.  Attach this script to the Panel.
///   3. Inside the Panel add four TMP_Text children:
///        • LevelNameText   — e.g. "Entering: The Graveyard"
///        • PromptText      — e.g. "Press E to Board the Boat"
///        • GoldText        — e.g. "Gold: 120"
///        • CharLevelText   — e.g. "Character Level: 3  (XP: 45 / 225)"
///   4. Wire up the four references in the Inspector.
///   5. Drag this Panel into DockBoat → Level Transition Panel.
/// </summary>
public class LevelTransitionPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text levelNameText;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text charLevelText;

    [Header("Config")]
    [Tooltip("Override display names per level (index 0 = level 1). Leave empty to use PlayerData.LevelDisplayNames.")]
    [SerializeField] private string[] levelDisplayNames;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    /// <summary>Show the panel and refresh all displayed stats.</summary>
    public void Show()
    {
        Refresh();
        gameObject.SetActive(true);
    }

    /// <summary>Hide the panel.</summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>Rebuild all text fields from the current PlayerData snapshot.</summary>
    public void Refresh()
    {
        PlayerData pd = PlayerData.GetOrCreate();

        // Level name
        string[] names = (levelDisplayNames != null && levelDisplayNames.Length > 0)
            ? levelDisplayNames
            : PlayerData.LevelDisplayNames;

        int index = Mathf.Clamp(pd.currentLevel - 1, 0, names.Length - 1);
        string displayName = names[index];

        if (levelNameText != null)
            levelNameText.text = $"Entering Level {pd.currentLevel}: {displayName}";

        // Prompt
        if (promptText != null)
            promptText.text = "Press E to Board the Boat";

        // Gold
        if (goldText != null)
            goldText.text = $"Gold:  {pd.gold}";

        // Character level + XP
        if (charLevelText != null)
            charLevelText.text = $"Character Level:  {pd.playerLevel}   (XP: {pd.xp} / {pd.xpToNextLevel})";
    }
}
