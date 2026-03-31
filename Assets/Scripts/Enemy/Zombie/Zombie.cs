using UnityEngine;
using System.Collections;

/// Zombie enemy. Extends Entity for health/money.
/// On death: drops coins into the player's Entity.money and awards XP via PlayerInventory.
/// Stats are driven by a ZombieData ScriptableObject so each level can tune them
/// without touching code.

public class Zombie : Entity
{
    [Header("Zombie Settings")]
    [SerializeField] private ZombieData data;
    [SerializeField] private Transform[] patrolPoints;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;

    // State machine 

    private enum ZombieState { Patrol, Chase, Attack, Dead }
    private ZombieState state = ZombieState.Patrol;

    // Cached refs 

    private Transform playerTransform;
    private Player playerEntity;               // For GainXP / GainCoins helpers
    private int patrolIndex = 0;
    private float lastAttackTime = -Mathf.Infinity;

    
    // Stat shortcuts 

    private int   AttackDamage   => data != null ? data.attackDamage   : 10;
    private float MoveSpeed      => data != null ? data.moveSpeed      : 2f;
    private float DetectRange    => data != null ? data.detectRange    : 6f;
    private float AttackRange    => data != null ? data.attackRange    : 1f;
    private float AttackCooldown => data != null ? data.attackCooldown : 1.5f;
    private int   CoinDropMin    => data != null ? data.coinDropMin    : 1;
    private int   CoinDropMax    => data != null ? data.coinDropMax    : 5;
    private int   XPReward       => data != null ? data.xpReward      : 20;

    // Lifecycle 

    protected override void Awake()
    {
        base.Awake();
        if (data != null)
        {
            maxHealth     = data.maxHealth;
            currentHealth = maxHealth;
        }
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerEntity    = playerObj.GetComponent<Player>();
        }
    }

    private void Update()
    {
        if (!IsAlive) return;
        switch (state)
        {
            case ZombieState.Patrol: HandlePatrol(); break;
            case ZombieState.Chase:  HandleChase();  break;
            case ZombieState.Attack: HandleAttack(); break;
        }
    }

    // State handlers 

    private void HandlePatrol()
    {
        if (playerTransform != null &&
            Vector2.Distance(transform.position, playerTransform.position) <= DetectRange)
        {
            state = ZombieState.Chase;
            return;
        }

        if (patrolPoints == null || patrolPoints.Length == 0) return;

        MoveTowards(patrolPoints[patrolIndex].position);
        if (Vector3.Distance(transform.position, patrolPoints[patrolIndex].position) < 0.2f)
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
    }

    private void HandleChase()
    {
        if (playerTransform == null) { state = ZombieState.Patrol; return; }

        float dist = Vector3.Distance(transform.position, playerTransform.position);

        if (dist > DetectRange * 1.5f) { state = ZombieState.Patrol; return; }
        if (dist <= AttackRange)       { state = ZombieState.Attack;  return; }

        MoveTowards(playerTransform.position);
    }

    private void HandleAttack()
    {
        if (playerTransform == null) { state = ZombieState.Patrol; return; }

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist > AttackRange) { state = ZombieState.Chase; return; }

        if (Time.time - lastAttackTime >= AttackCooldown)
        {
            lastAttackTime = Time.time;
            playerEntity?.TakeDamage(AttackDamage);     // TakeDamage is on Entity
        }
    }

    private void MoveTowards(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        rb.linearVelocity = new Vector3(dir.x * MoveSpeed, rb.linearVelocity.y, dir.z * MoveSpeed);
        if (dir.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(dir.x), 1f, 1f);
    }

    // Entity implementation 

    protected override void OnDamageTaken(int amount)
    {
        // TODO: hit flash / sound
    }

    protected override void OnDeath()
    {
        state = ZombieState.Dead;
        rb.linearVelocity = Vector3.zero;

        // Give coins directly into Entity.money on the player
        int coinsToDrop = Random.Range(CoinDropMin, CoinDropMax + 1);
        playerEntity?.AddMoney(coinsToDrop);    // AddMoney lives in Entity

        // Give XP via PlayerInventory
        playerEntity?.GainXP(XPReward);

        StartCoroutine(DestroyAfterDelay(2f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}