using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Attach to the NPC that must be kept alive in your level.
/// If the NPC's health reaches 0, the level restarts.
/// </summary>
public class ProtectedNPC : Entity
{
    [Tooltip("Seconds after death before reloading the scene.")]
    [SerializeField] private float failDelay = 2f;

    [Tooltip("Optional UI panel to show on failure (e.g. 'The NPC has fallen!').")]
    [SerializeField] private GameObject failPanel;

    protected override void OnDamageTaken(int amount)
    {
        Debug.Log($"[ProtectedNPC] Took {amount} damage. HP: {currentHealth}/{maxHealth}");
    }

    protected override void OnDeath()
    {
        Debug.Log("[ProtectedNPC] The NPC has fallen — level failed.");

        if (failPanel != null)
            failPanel.SetActive(true);

        StartCoroutine(ReloadAfterDelay());
    }

    private System.Collections.IEnumerator ReloadAfterDelay()
    {
        yield return new WaitForSeconds(failDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
