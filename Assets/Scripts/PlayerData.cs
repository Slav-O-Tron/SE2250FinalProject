using UnityEngine;
using System.Collections.Generic;


/// Persistent singleton that tracks cross-scene game progression.
/// Attach to a GameObject named "PlayerData" in the first scene (StartScreen/MainMenu).
/// Survives all scene loads via DontDestroyOnLoad.

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }

    // ── Game-world progress ───────────────────────────────────────────────────
    public int currentLevel = 1;
    public bool hasSeenMerchantChronosphereIntro = false;
    public const int MaxLevels = 5;

    // ── Player currency & character progression ───────────────────────────────
    /// <summary>Gold carried by the player across scenes.</summary>
    public int gold = 0;
    /// <summary>Current experience points (within the current character level).</summary>
    public int xp = 0;
    /// <summary>Player character level (skill level, not the game world level).</summary>
    public int playerLevel = 1;
    /// <summary>XP required to reach the next character level.</summary>
    public int xpToNextLevel = 100;

    // ── Scene → display name mapping ─────────────────────────────────────────

    /// Human-readable names for each game-world level (index 0 = level 1).
    /// Matches the order set in DockBoat.levelSceneNames.    /// </summary>
    public static readonly string[] LevelDisplayNames =
    {
        "The Village",
        "The Graveyard",
        "The Wasteland",
        "The Dark Sanctum",
        "The Final Realm"
    };

    // ── Persistent inventory ──────────────────────────────────────────────────
    [System.Serializable]
    public class InventorySaveEntry
    {
        public ItemData item;
        public int quantity;
    }
    public List<InventorySaveEntry> savedInventory = new List<InventorySaveEntry>();

    // ── Persistent equipped items ─────────────────────────────────────────────
    public List<ItemData> savedEquipped = new List<ItemData>();

    // ── Persistent health ─────────────────────────────────────────────────────
    /// -1 means "not yet set — start at full health".
    public int savedHealth = -1;

    private readonly HashSet<int> collectedChronospherePieces = new HashSet<int>();

    public int ChronospherePieceCount => collectedChronospherePieces.Count;
    public bool HasCompletedChronosphere() => ChronospherePieceCount >= MaxLevels;


    /// Called by any script that needs PlayerData before it may have been
    /// created (e.g. entering a level directly in the editor).
    /// Creates a runtime instance automatically if none exists yet.

    public static PlayerData GetOrCreate()
    {
        if (Instance != null) return Instance;

        GameObject go = new GameObject("PlayerData");
        return go.AddComponent<PlayerData>(); // Awake runs immediately and sets Instance
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    /// Increment currentLevel after a level is completed.
    /// Returns true if more levels remain, false if the game is won (was on level 5).

    public bool AdvanceLevel()
    {
        currentLevel++;
        return currentLevel <= MaxLevels;
    }

    public bool HasPieceForLevel(int level)
    {
        return collectedChronospherePieces.Contains(level);
    }

    public bool CollectPieceForLevel(int level)
    {
        if (level < 1 || level > MaxLevels)
            return false;

        return collectedChronospherePieces.Add(level);
    }

    public bool IsGameWon() => currentLevel > MaxLevels;

    /// Reset back to level 1, e.g. on New Game.
    public void ResetProgress()
    {
        currentLevel = 1;
        hasSeenMerchantChronosphereIntro = false;
        collectedChronospherePieces.Clear();
    }
}
