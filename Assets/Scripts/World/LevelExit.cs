using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Attach to the exit boat in a level scene.
/// Stays locked (E does nothing) until LevelCompletion.CompleteLevel() calls Unlock().
/// Once unlocked, player presses E to return to MainWorld.
/// </summary>
public class LevelExit : MonoBehaviour
{
    [Tooltip("Text shown near the boat before the level is complete.")]
    [SerializeField] private string lockedMessage = "Complete the objective first!";
    [Tooltip("Scene to load when the full Chronosphere is restored.")]
    [SerializeField] private string gameWonSceneName = "MainMenu";

    private bool isUnlocked = false;
    private bool playerInRange = false;
    private HUD hud;
    private LevelCompletion levelCompletion;

    private void Start()
    {
        hud = FindFirstObjectByType<HUD>();
        levelCompletion = FindFirstObjectByType<LevelCompletion>();
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isUnlocked)
            {
                PlayerData playerData = PlayerData.GetOrCreate();

                // If the level was never completed through normal flow, advance the level now
                // so DockBoat loads the correct next level when back in MainWorld.
                bool completedNormally = levelCompletion != null && levelCompletion.RewardClaimed;
                if (!completedNormally)
                    playerData.AdvanceLevel();

                string sceneName = playerData.HasCompletedChronosphere() ? gameWonSceneName : "MainWorld";
                SceneManager.LoadScene(sceneName);
            }
            else
                Debug.Log($"[LevelExit] {GetLockedMessage()}");
        }
    }

    /// <summary>Called by LevelCompletion once all objectives are met.</summary>
    public void Unlock()
    {
        isUnlocked = true;
        // Refresh the prompt if the player is already standing at the boat
        if (playerInRange)
            hud?.ShowInteractPrompt("Press E to return to the Main World");
        Debug.Log("[LevelExit] Exit boat unlocked.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;

        if (isUnlocked)
            hud?.ShowInteractPrompt("Press E to return to the Main World");
        else
            hud?.ShowInteractPrompt(GetLockedMessage());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        hud?.HideInteractPrompt();
    }

    private string GetLockedMessage()
    {
        if (levelCompletion != null && levelCompletion.IsComplete && !levelCompletion.RewardClaimed)
            return "Speak with the Elder first!";

        return lockedMessage;
    }
}
