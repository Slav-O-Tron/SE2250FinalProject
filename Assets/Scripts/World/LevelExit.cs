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
    private GameObject interactPrompt;
    private HUD hud;

    private void Start()
    {
        hud = FindFirstObjectByType<HUD>();
        if (hud != null) interactPrompt = hud.interactPrompt;
        if (interactPrompt != null) interactPrompt.SetActive(false);
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
        if (playerInRange && interactPrompt != null)
            interactPrompt.SetActive(true);
        Debug.Log("[LevelExit] Exit boat unlocked.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;

        if (isUnlocked && interactPrompt != null)
            interactPrompt.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }
}
