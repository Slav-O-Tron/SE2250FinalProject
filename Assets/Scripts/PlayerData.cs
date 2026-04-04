using UnityEngine;


/// Persistent singleton that tracks cross-scene game progression.
/// Attach to a GameObject named "PlayerData" in the first scene (StartScreen/MainMenu).
/// Survives all scene loads via DontDestroyOnLoad.

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }

    public int currentLevel = 1;
    public const int MaxLevels = 5;

    /// <summary>
    /// Called by any script that needs PlayerData before it may have been
    /// created (e.g. entering a level directly in the editor).
    /// Creates a runtime instance automatically if none exists yet.
    /// </summary>
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

    public bool IsGameWon() => currentLevel > MaxLevels;

    /// <summary>Reset back to level 1, e.g. on New Game.</summary>
    public void ResetProgress()
    {
        currentLevel = 1;
    }
}
