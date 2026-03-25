using UnityEngine;
using TMPro;
using UnityEngine.UI;


/// Reads stats from Player (Entity) and PlayerInventory each frame and
/// pushes them to UI elements. Assign all references in the Inspector.
///
/// Scene setup:
///   Canvas (Screen Space - Overlay)
///   └── HUD (empty GameObject, attach this script)
///       ├── HealthBar    (Slider)
///       ├── HealthText   (TMP_Text)   e.g. "80 / 100"
///       ├── StaminaBar   (Slider)
///       ├── CoinsText    (TMP_Text)   e.g. "Coins: 42"
///       ├── XPBar        (Slider)
///       └── LevelText    (TMP_Text)   e.g. "Level 3"

public class HUD : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Player player;                 // Drag your Player GameObject here
    [SerializeField] private PlayerInventory inventory;     // Same GameObject as Player

    [Header("Health")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TMP_Text healthText;           // Optional "80 / 100" label

    [Header("Stamina")]
    [SerializeField] private Slider staminaBar;

    [Header("Coins")]
    [SerializeField] private TMP_Text coinsText;

    [Header("XP / Level")]
    [SerializeField] private Slider xpBar;
    [SerializeField] private TMP_Text levelText;

    private void Start()
    {
        // Auto-find if not assigned in Inspector
        if (player == null)
            player = FindFirstObjectByType<Player>();

        if (player != null && inventory == null)
            inventory = player.GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        if (player == null || inventory == null) return;

        UpdateHealth();
        UpdateStamina();
        UpdateCoins();
        UpdateXP();
    }

    private void UpdateHealth()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = player.MaxHealth;
            healthBar.value    = player.CurrentHealth;
        }

        if (healthText != null)
            healthText.text = $"{player.CurrentHealth} / {player.MaxHealth}";
    }

    private void UpdateStamina()
    {
        if (staminaBar != null)
        {
            staminaBar.maxValue = inventory.maxStamina;
            staminaBar.value    = inventory.stamina;
        }
    }

    private void UpdateCoins()
    {
        if (coinsText != null)
            coinsText.text = $"Coins: {player.Money}";     // Money lives on Entity
    }

    private void UpdateXP()
    {
        if (xpBar != null)
        {
            xpBar.maxValue = inventory.xpToNextLevel;
            xpBar.value    = inventory.xp;
        }

        if (levelText != null)
            levelText.text = $"Level {inventory.level}";
    }
}
