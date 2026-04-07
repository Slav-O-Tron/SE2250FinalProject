using UnityEngine;

/// Sits alongside Player on the same GameObject.
/// Owns: Stamina, XP/Level.
/// Health and Money are owned by Entity (the parent class of Player).

public class PlayerInventory : MonoBehaviour
{
    public bool hasDoorKey = false;

    public int xp = 0;
    public int level = 1;
    public int xpToNextLevel = 100;

    public float maxStamina = 100f;
    public float stamina = 100f;
    public float staminaRegenRate = 10f;    // Per second when not sprinting
    public float staminaRegenDelay = 1.5f;  // Seconds after sprint stops before regen

    private float regenDelayTimer = 0f;

    private void Awake()
    {
        PlayerData pd = PlayerData.GetOrCreate();
        xp = pd.xp;
        level = pd.playerLevel;
        xpToNextLevel = pd.xpToNextLevel;
    }

    private void OnDestroy()
    {
        SaveToPlayerData();
    }

    /// <summary>Persist current XP and level into PlayerData so they survive scene loads.</summary>
    public void SaveToPlayerData()
    {
        PlayerData pd = PlayerData.GetOrCreate();
        pd.xp = xp;
        pd.playerLevel = level;
        pd.xpToNextLevel = xpToNextLevel;
    }

    private void Update()
    {
        if (regenDelayTimer > 0f)
            regenDelayTimer -= Time.deltaTime;
        else if (stamina < maxStamina)
            stamina = Mathf.Min(maxStamina, stamina + staminaRegenRate * Time.deltaTime);
    }

    // XP 

    public void AddXP(int amount)
    {
        xp += amount;
        while (xp >= xpToNextLevel)
        {
            xp -= xpToNextLevel;
            level++;
            xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f);
            OnLevelUp();
        }
        Debug.Log($"XP: {xp}/{xpToNextLevel}  Level: {level}");
        SaveToPlayerData();
    }

    private void OnLevelUp()
    {
        maxStamina += 10f;
        stamina = maxStamina;
        Debug.Log($"Level up! Now level {level}");
    }

    // Stamina 

    /// Drain stamina by amount. Returns false when empty so Player.cs
    /// automatically falls back to walk speed.
    public bool UseStamina(float amount)
    {
        if (stamina <= 0f) return false;
        stamina = Mathf.Max(0f, stamina - amount);
        regenDelayTimer = staminaRegenDelay;
        return true;
    }

    public bool HasStamina() => stamina > 0f;

    // Item check 


    /// Returns true if the player's inventory contains at least one of the named item.
    /// Use this for puzzle/door checks instead of adding a new bool each time.
    public bool HasItem(string itemName)
    {
        InventoryManager inventoryManager = GameObject.Find("InventoryCanvas")
            ?.GetComponent<InventoryManager>();

        if (inventoryManager == null) return false;

        foreach (ItemSlot slot in inventoryManager.itemSlot)
            if (slot.isFull && slot.itemName == itemName)
                return true;

        return false;
    }
}