using UnityEngine;

public class DemonBoss : EnemyMotion
{
    private Animator anim;
    private Transform player;

    [Header("Boss Settings")]
    public float attackRange = 3.5f;
    public float attackCooldown = 2.0f;
    private float lastAttackTime;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();

        
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("DemonBoss: No GameObject with the 'Player' tag found in the scene!");
        }

        if (agent != null)
        {
            agent.speed = 7f;
            agent.acceleration = 12f;
            agent.stoppingDistance = 2.5f;
        }
    }

    protected override void Update()
    {
        base.Update();

        
        if (anim == null || player == null || agent == null) return;

        float currentSpeed = agent.velocity.magnitude;

        
        anim.SetFloat("Speed", currentSpeed);

        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            PerformRandomAttack();
        }
    }

    void PerformRandomAttack()
    {
        lastAttackTime = Time.time;

        float choice = Random.value;

        if (choice < 0.5f)
        {
            anim.SetTrigger("BasicAttack");
        }
        else if (choice < 0.8f)
        {
            anim.SetTrigger("HeavyAttack");
        }
        else
        {
            anim.SetTrigger("SpecialMove");
        }

        agent.isStopped = true;
        Invoke("ResumeMovement", 1.5f);
    }

    void ResumeMovement()
    {
        if (agent != null && agent.enabled) agent.isStopped = false;
    }
}