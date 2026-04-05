using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Attach to the Boat GameObject in the Main World Hub.
/// Requires a Trigger Collider on the same object.
///
/// When the player presses E while in range, the boat loads the scene
/// matching the player's current level: "Level_1", "Level_2", etc.
/// </summary>
public class DockBoat : MonoBehaviour
{
    private HUD hud;
    private bool playerInRange = false;

    private void Start()
    {
        hud = FindFirstObjectByType<HUD>();
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TakeBoat();
        }
    }

    [Tooltip("Override scene names per level. Index 0 = Level 1, index 1 = Level 2, etc.")]
    [SerializeField] private string[] levelSceneNames = { "Level_1", "Level_2", "Level_3", "Level_4", "Level_5" };

    private void TakeBoat()
    {
        int level = PlayerData.GetOrCreate().currentLevel;
        int index = Mathf.Clamp(level - 1, 0, levelSceneNames.Length - 1);
        string sceneName = levelSceneNames[index];
        Debug.Log($"[DockBoat] Attempting to load scene: '{sceneName}'");
        SceneManager.LoadScene(sceneName);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[DockBoat] Trigger entered by: {other.gameObject.name} (tag: {other.tag})");
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            hud?.ShowInteractPrompt("Press E to take the boat");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            hud?.HideInteractPrompt();
        }
    }
}
