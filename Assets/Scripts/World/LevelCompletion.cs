using UnityEngine;

/// <summary>
/// Place one instance of this in every Level scene.
/// Called by WaveManager or BossEnemy when the objective is complete.
/// Does NOT auto-load any scene — instead it unlocks the LevelExit boat
/// so the player can walk back to it and leave manually.
/// </summary>
public class LevelCompletion : MonoBehaviour
{
    [Tooltip("Optional UI panel to show when all waves are cleared (e.g. 'Level Complete! Return to the boat.').")]
    [SerializeField] private GameObject levelCompletePanel;

    private bool levelCompleted = false;

    public bool IsComplete => levelCompleted;

    /// <summary>
    /// Called by WaveManager / BossEnemy when the objective is met.
    /// Advances PlayerData and unlocks the exit boat.
    /// </summary>
    public void CompleteLevel()
    {
        if (levelCompleted) return;
        levelCompleted = true;

        PlayerData.GetOrCreate().AdvanceLevel();

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);

        // Notify the exit boat in this scene that it is now usable
        LevelExit exit = FindFirstObjectByType<LevelExit>();
        if (exit != null)
            exit.Unlock();

        Debug.Log("[LevelCompletion] Level complete — exit unlocked.");
    }
}
