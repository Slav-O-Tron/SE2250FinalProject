using UnityEngine;


/// Attach to a Boss enemy in a level scene alongside (or instead of) EnemyHealth.
/// When this boss is killed it calls LevelCompletion.CompleteLevel(), ending the level.


public class BossEnemy : Entity
{
    [SerializeField] private int deathDestroyDelay = 1;

    private LevelCompletion levelCompletion;
    private bool isDead = false;

    protected override void Awake()
    {
        base.Awake();
        levelCompletion = FindFirstObjectByType<LevelCompletion>();
    }

    protected override void OnDamageTaken(int amount)
    {
        Debug.Log($"[Boss] Took {amount} damage. HP: {currentHealth}/{maxHealth}");
    }

    protected override void OnDeath()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[Boss] Defeated!");
        levelCompletion?.CompleteLevel();
        Destroy(gameObject, deathDestroyDelay);
    }
}
