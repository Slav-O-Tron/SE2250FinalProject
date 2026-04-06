using UnityEngine;

/// <summary>
/// Place one instance of this in every Level scene.
/// Called by WaveManager or BossEnemy when the objective is complete.
/// Does NOT auto-load any scene — instead it unlocks the LevelExit boat
/// so the player can walk back to it and leave manually.
/// </summary>
public class LevelCompletion : MonoBehaviour
{
    [Tooltip("Optional UI panel to show when all waves are cleared (e.g. 'Speak with the Elder').")]
    [SerializeField] private GameObject levelCompletePanel;

    private bool levelCompleted = false;
    private bool rewardClaimed = false;

    public bool IsComplete => levelCompleted;
    public bool RewardClaimed => rewardClaimed;

    /// <summary>
    /// Called by WaveManager / BossEnemy when the objective is met.
    /// Marks the level objective complete, then asks the elder to hand over the piece.
    /// </summary>
    public void CompleteLevel()
    {
        if (levelCompleted) return;
        levelCompleted = true;

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);

        HUD hud = FindFirstObjectByType<HUD>();
        hud?.SetDefaultPrompt("Speak with the Elder to claim the Chronosphere piece.");

        StoryNPC elder = FindFirstObjectByType<StoryNPC>();
        if (elder != null)
        {
            elder.PrepareChronosphereReward();
        }
        else
        {
            ClaimChronosphereReward();
        }

        Debug.Log("[LevelCompletion] Level objective complete — waiting for Chronosphere reward.");
    }

    public bool ClaimChronosphereReward()
    {
        if (!levelCompleted || rewardClaimed)
            return false;

        rewardClaimed = true;

        PlayerData playerData = PlayerData.GetOrCreate();
        int pieceLevel = playerData.currentLevel;
        bool addedNewPiece = playerData.CollectPieceForLevel(pieceLevel);
        playerData.AdvanceLevel();

        HUD hud = FindFirstObjectByType<HUD>();
        if (playerData.HasCompletedChronosphere())
            hud?.SetDefaultPrompt("Chronosphere restored. Return to the boat.");
        else
            hud?.SetDefaultPrompt("Return to the boat.");

        LevelExit exit = FindFirstObjectByType<LevelExit>();
        if (exit != null)
            exit.Unlock();

        Debug.Log($"[LevelCompletion] Chronosphere piece claimed for level {pieceLevel}. Total pieces: {playerData.ChronospherePieceCount}. New piece: {addedNewPiece}");
        return true;
    }
}
