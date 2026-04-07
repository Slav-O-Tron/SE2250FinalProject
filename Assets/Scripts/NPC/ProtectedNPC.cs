using UnityEngine;

/// <summary>
/// Attach to the NPC that must be kept alive in your level.
/// On death, triggers LevelFailManager to show the fail screen and stop the waves.
/// </summary>
public class ProtectedNPC : Entity
{
    protected override void OnDamageTaken(int amount)
    {
        Debug.Log($"[ProtectedNPC] Took {amount} damage. HP: {currentHealth}/{maxHealth}");
    }

    protected override void OnDeath()
    {
        Debug.Log("[ProtectedNPC] The NPC has fallen — level failed.");
        LevelFailManager failManager = LevelFailManager.Instance != null
            ? LevelFailManager.Instance
            : FindFirstObjectByType<LevelFailManager>();
        if (failManager != null)
            failManager.TriggerFail("The Elder has fallen!");
        else
            Debug.LogError("[ProtectedNPC] No LevelFailManager found in scene. Add a LevelFailManager GameObject to this level.");
    }
}
