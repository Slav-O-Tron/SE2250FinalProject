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

    private bool isUnlocked = false;
    private bool playerInRange = false;
    private HUD hud;

    private void Start()
    {
        hud = FindFirstObjectByType<HUD>();
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isUnlocked)
                SceneManager.LoadScene("MainWorld");
            else
                Debug.Log($"[LevelExit] {lockedMessage}");
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
            hud?.ShowInteractPrompt(lockedMessage);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        hud?.HideInteractPrompt();
    }
}
