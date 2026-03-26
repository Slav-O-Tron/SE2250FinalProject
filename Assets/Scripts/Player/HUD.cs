using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Player player;
    [SerializeField] private PlayerInventory inventory;

    [Header("Health")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TMP_Text healthText;

    [Header("Stamina")]
    [SerializeField] private Slider staminaBar;
    [SerializeField] private TMP_Text staminaText;

    [Header("Coins")]
    [SerializeField] private TMP_Text coinsText;

    [Header("XP / Level")]
    [SerializeField] private Slider xpBar;
    [SerializeField] private TMP_Text levelText;

    [Header("Interact Prompt")]
    public GameObject interactPrompt;

    [Header("HUD Panel")]
    public GameObject hudPanel;

    public void ShowHUD(bool show)
    {
        if (hudPanel != null) hudPanel.SetActive(show);
    }

    private void Awake()
    {
        RebindPlayer();
    }

    private void Start()
    {
        RebindPlayer();
        RefreshAll();
    }

    private void Update()
    {
        if (player == null)
            RebindPlayer();

        if (player == null)
            return;

        UpdateHealth();
        UpdateCoins();

        if (inventory == null)
            inventory = player.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            UpdateStamina();
            UpdateXP();
        }
    }

    private void RebindPlayer()
    {
        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);

        foreach (Player p in players)
        {
            if (!p.gameObject.activeInHierarchy) continue;

            player = p;
            inventory = p.GetComponent<PlayerInventory>();
            break;
        }
    }

    private void RefreshAll()
    {
        if (player == null) return;

        UpdateHealth();
        UpdateCoins();

        if (inventory != null)
        {
            UpdateStamina();
            UpdateXP();
        }
    }

    private void UpdateHealth()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = player.MaxHealth;
            healthBar.value = player.CurrentHealth;
        }

        if (healthText != null)
            healthText.text = $"{player.CurrentHealth} / {player.MaxHealth}";
    }

    private void UpdateStamina()
    {
        if (staminaBar != null)
        {
            staminaBar.maxValue = inventory.maxStamina;
            staminaBar.value = inventory.stamina;
        }

        if (staminaText != null)
            staminaText.text = $"{inventory.stamina} / {inventory.maxStamina}";
    }

    private void UpdateCoins()
    {
        if (coinsText != null)
            coinsText.text = $"Coins: {player.Money}";
    }

    private void UpdateXP()
    {
        if (xpBar != null)
        {
            xpBar.maxValue = inventory.xpToNextLevel;
            xpBar.value = inventory.xp;
        }

        if (levelText != null)
            levelText.text = $"Level {inventory.level}";
    }
}