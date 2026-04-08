using UnityEngine;
using System.Collections;

/// Zombie enemy for Tomas's level. Uses Rigidbody for movement (no NavMesh required).
/// Always chases whichever target (Player or NPC tagged "NPC") is closest.
/// Damages that target when within attack range.
/// On death: awards coins and XP to the player.

public class TomasZombie : Entity
{
    [Header("Zombie Settings")]
    [SerializeField] private ZombieData data;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;

    // State machine

    private enum ZombieState { Chase, Attack, Dead }
    private ZombieState state = ZombieState.Chase;

    // Cached refs

    private Player[]  playerTargets;
    private Entity[]  npcEntities;

    private Transform targetTransform;
    private Entity targetEntity;

    private float lastAttackTime = -Mathf.Infinity;
    private float nextTargetRefreshTime = -Mathf.Infinity;

    // Stat shortcuts

    private int   AttackDamage   => data != null ? data.attackDamage   : 10;
    private float MoveSpeed      => data != null ? data.moveSpeed      : 2f;
    private float AttackRange    => data != null ? data.attackRange    : 1f;
    private float AttackCooldown => data != null ? data.attackCooldown : 1.5f;
    private int   CoinDropMin    => data != null ? data.coinDropMin    : 1;
    private int   CoinDropMax    => data != null ? data.coinDropMax    : 5;
    private int   XPReward       => data != null ? data.xpReward      : 20;

    private const float TargetRefreshInterval = 0.25f;

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
        RefreshTargets(true);
    }

    private void Update()
    {
        if (animator != null && rb != null)
            animator.SetFloat("Speed", new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude);

        if (!IsAlive) return;

        RefreshTargets();

        switch (state)
        {
            case ZombieState.Chase:  HandleChase();  break;
            case ZombieState.Attack: HandleAttack(); break;
        }
    }

    // State handlers

    private void HandleChase()
    {
        targetTransform = GetClosestTarget(out targetEntity);

        if (targetTransform == null)
        {
            StopMoving();
            return;
        }

        float dist = Vector3.Distance(transform.position, targetTransform.position);

        if (dist <= AttackRange)
        {
            StopMoving();
            state = ZombieState.Attack;
            return;
        }

        MoveTowards(targetTransform.position);
    }

    private void HandleAttack()
    {
        targetTransform = GetClosestTarget(out targetEntity);

        if (targetTransform == null) { state = ZombieState.Chase; return; }

        float dist = Vector3.Distance(transform.position, targetTransform.position);
        if (dist > AttackRange) { state = ZombieState.Chase; return; }

        // Face the target while attacking
        Vector3 dir = targetTransform.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);

        if (Time.time - lastAttackTime >= AttackCooldown)
        {
            lastAttackTime = Time.time;
            if (animator != null) animator.SetTrigger("Attack");
            if (targetEntity != null) targetEntity.TakeDamage(AttackDamage);
        }
    }

    // Movement

    private void MoveTowards(Vector3 target)
    {
        Vector3 offset = target - transform.position;
        offset.y = 0;

        if (offset.sqrMagnitude > 0.3f)
        {
            Vector3 moveDir = offset.normalized;
            rb.linearVelocity = new Vector3(moveDir.x * MoveSpeed, rb.linearVelocity.y, moveDir.z * MoveSpeed);

            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
        else
        {
            StopMoving();
        }
    }

    private void StopMoving()
    {
        if (rb != null)
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
    }

    // Target selection: closest live Player or NPC

    private void RefreshTargets(bool force = false)
    {
        if (!force && Time.time < nextTargetRefreshTime)
            return;

        nextTargetRefreshTime = Time.time + TargetRefreshInterval;
        RefreshPlayerReference();
        RefreshNpcReferences();
    }

    private void RefreshPlayerReference()
    {
        Player activePlayer = Player.ResolveActivePlayer();
        playerTargets = activePlayer != null
            ? new[] { activePlayer }
            : FindObjectsByType<Player>(FindObjectsSortMode.None);
    }

    private void RefreshNpcReferences()
    {
        GameObject[] npcObjects = GameObject.FindGameObjectsWithTag("NPC");
        npcEntities = new Entity[npcObjects.Length];

        for (int i = 0; i < npcObjects.Length; i++)
            npcEntities[i] = npcObjects[i].GetComponent<Entity>();
    }

    private Transform GetClosestTarget(out Entity closestEntity)
    {
        Transform closest = null;
        closestEntity = null;
        float closestDist = Mathf.Infinity;

        if (playerTargets != null)
        {
            foreach (Player p in playerTargets)
            {
                if (p == null || !p.IsAlive) continue;
                float d = Vector3.Distance(transform.position, p.transform.position);
                if (d < closestDist)
                {
                    closestDist   = d;
                    closest       = p.transform;
                    closestEntity = p;
                }
            }
        }

        if (npcEntities != null)
        {
            foreach (Entity npc in npcEntities)
            {
                if (npc == null || !npc.IsAlive) continue;
                float d = Vector3.Distance(transform.position, npc.Position);
                if (d < closestDist)
                {
                    closestDist   = d;
                    closest       = npc.transform;
                    closestEntity = npc;
                }
            }
        }

        return closest;
    }

    // Entity implementation

    protected override void OnDamageTaken(int amount)
    {
        // Entity.TakeDamage already decremented currentHealth and will call OnDeath.
        // Only log here; do not touch health or call OnDeath again.
        Debug.Log($"{gameObject.name} took {amount} damage. HP: {currentHealth}/{maxHealth}");
    }

    protected override void OnDeath()
    {
        state = ZombieState.Dead;
        StopMoving();
        GetComponent<ChronoShardCarrier>()?.Drop();

        int coinsToDrop = Random.Range(CoinDropMin, CoinDropMax + 1);
        RefreshPlayerReference();
        if (playerTargets != null && playerTargets.Length > 0)
        {
            playerTargets[0].AddMoney(coinsToDrop);
            playerTargets[0].GainXP(XPReward);
        }

        StartCoroutine(DestroyAfterDelay(2f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
