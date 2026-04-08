using UnityEngine;
using System;


/// Place one instance of this in every Level scene.
/// Called by WaveManager or BossEnemy when the objective is complete.
/// Does NOT auto-load any scene — instead it unlocks the LevelExit boat
/// so the player can walk back to it and leave manually.

public class LevelCompletion : MonoBehaviour
{
    [Tooltip("Optional UI panel to show when all waves are cleared (e.g. 'Speak with the Elder').")]
    [SerializeField] private GameObject levelCompletePanel;

    [Tooltip("The Chronosphere Piece ItemData ScriptableObject to add to the player's inventory on level completion.")]
    [SerializeField] private ItemData chronospherePieceItemData;

    [Tooltip("Optional shard ordering puzzle shown before the Chronosphere piece is awarded.")]
    [SerializeField] private ShardAssemblyPuzzlePanel shardAssemblyPuzzlePanel;

    private bool levelCompleted = false;
    private bool rewardClaimed = false;
    private bool shardAssemblySolved = false;

    public bool IsComplete => levelCompleted;
    public bool RewardClaimed => rewardClaimed;
    public bool HasPendingRewardPuzzle => shardAssemblyPuzzlePanel != null && !shardAssemblySolved;


    /// Called by WaveManager / BossEnemy when the objective is met.
    /// Marks the level objective complete, then asks the elder to hand over the piece.

    public void CompleteLevel()
    {
        if (levelCompleted) return;
        levelCompleted = true;

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);

        HUD hud = FindFirstObjectByType<HUD>();
        hud?.SetDefaultPrompt("The ritual is complete. Speak with the Elder.");

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

    public bool TryOpenRewardPuzzle(Action onSolved, Action onClosed = null)
    {
        if (rewardClaimed || shardAssemblyPuzzlePanel == null || shardAssemblySolved)
            return false;

        shardAssemblyPuzzlePanel.Show(
            solvedCallback: () =>
            {
                shardAssemblySolved = true;
                onSolved?.Invoke();
            },
            closedCallback: onClosed);

        return true;
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

        if (addedNewPiece && chronospherePieceItemData != null)
        {
            InventoryManager inventoryManager = FindFirstObjectByType<InventoryManager>();
            if (inventoryManager != null)
                inventoryManager.AddItem(chronospherePieceItemData, 1);
            else
                Debug.LogWarning("[LevelCompletion] InventoryManager not found — Chronosphere piece not added to inventory.");
        }

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
