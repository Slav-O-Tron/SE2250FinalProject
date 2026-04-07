using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Place one instance of this in every level scene.
/// Called by Player.OnDeath and ProtectedNPC.OnDeath to show the fail screen
/// and halt the wave system.
/// 
/// Inspector setup:
///   - failPanel      : the root UI panel to activate on failure
///   - failReasonText : (optional) TMP_Text inside the panel to show the reason
/// The panel should contain Retry and Main Menu buttons wired to RetryLevel() / GoToMainMenu().
/// </summary>
public class LevelFailManager : MonoBehaviour
{
    public static LevelFailManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject failPanel;
    [SerializeField] private TMP_Text   failReasonText;

    private bool hasFailed = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>
    /// Trigger the fail state. Safe to call multiple times — only fires once.
    /// </summary>
    public void TriggerFail(string reason = "")
    {
        if (hasFailed) return;
        hasFailed = true;

        // Stop all waves
        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        waveManager?.StopWaves();

        // Disable player movement and look
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
            player.enabled = false;

        // Show fail panel
        if (failPanel != null)
            failPanel.SetActive(true);

        if (failReasonText != null && !string.IsNullOrEmpty(reason))
            failReasonText.text = reason;

        // Unlock cursor so the player can click Retry / Menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        Debug.Log($"[LevelFailManager] Level failed: {reason}");
    }

    /// <summary>Called by the Retry button on the fail panel.</summary>
    public void RetryLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>Called by the Main Menu button on the fail panel.</summary>
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
