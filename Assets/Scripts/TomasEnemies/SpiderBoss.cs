using UnityEngine;
using System.Collections;

/// Spider boss for Level 1's final wave.
/// Add this alongside SpiderMotion on the spider prefab.
/// On death it awards coins/XP and calls LevelCompletion.CompleteLevel().
public class SpiderBoss : Entity
{
    [Header("Stats")]
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int coinReward = 50;
    [SerializeField] private int xpReward = 150;

    [Header("References")]
    [SerializeField] private Animator animator;
    [Tooltip("Exact name of the Attack trigger in the Animator controller.")]
    [SerializeField] private string attackTrigger = "Attack";

    private SpiderMotion motion;
    private Player player;
    private LevelCompletion levelCompletion;
    private float lastAttackTime = -Mathf.Infinity;
    private bool isDead = false;

    protected override void Awake()
    {
        base.Awake();
        motion = GetComponent<SpiderMotion>();
        levelCompletion = FindFirstObjectByType<LevelCompletion>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Start()
    {
        player = Player.ResolveActivePlayer() ?? FindFirstObjectByType<Player>();

        // Give SpiderMotion its chase target
        if (motion != null && player != null)
            motion.target = player.transform;
    }

    private void Update()
    {
        if (isDead) return;

        // Re-acquire player if lost (e.g. scene reload)
        if (player == null || !player.IsAlive)
        {
            player = Player.ResolveActivePlayer() ?? FindFirstObjectByType<Player>();
            return;
        }

        float dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            if (animator != null && !string.IsNullOrEmpty(attackTrigger))
                animator.SetTrigger(attackTrigger);

            player.TakeDamage(attackDamage);
            Debug.Log($"[SpiderBoss] Attacked player for {attackDamage} damage.");
        }
    }

    protected override void OnDamageTaken(int amount)
    {
        Debug.Log($"[SpiderBoss] Took {amount} damage. HP: {currentHealth}/{maxHealth}");
    }

    protected override void OnDeath()
    {
        if (isDead) return;
        isDead = true;

        // Stop movement
        if (motion != null)
            motion.enabled = false;

        // Reward player
        Player p = Player.ResolveActivePlayer() ?? FindFirstObjectByType<Player>();
        if (p != null)
        {
            p.AddMoney(coinReward);
            p.GainXP(xpReward);
        }

        Debug.Log("[SpiderBoss] Defeated! Completing level.");
        levelCompletion?.CompleteLevel();

        StartCoroutine(DestroyAfterDelay(2f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
