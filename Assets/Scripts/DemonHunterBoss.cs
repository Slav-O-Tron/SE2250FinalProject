using UnityEngine;

public class DemonBoss : MonoBehaviour
{
    public Transform target;
    private Animator anim;
    private float moveSpeed = 5f;
    private float rotationSpeed = 5f;

    [Header("Boss Settings")]
    public float attackRange = 3.5f;
    public float attackCooldown = 2.0f;
    private float lastAttackTime;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (target == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
                Debug.Log("✓ DemonBoss FOUND player at: " + target.position);
            }
            else
            {
                Debug.LogError("✗ DemonBoss FAILED to find Player tag!");
            }
        }
        else
        {
            Debug.Log("✓ DemonBoss target already set to: " + target.name);
        }
    }

    void Update()
    {
        if (anim == null || target == null)
        {
            Debug.LogWarning("DemonBoss missing anim or target!");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        Vector3 directionToPlayer = (target.position - transform.position).normalized;
        transform.position += directionToPlayer * moveSpeed * Time.deltaTime;

        if (directionToPlayer.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(directionToPlayer.x, directionToPlayer.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        anim.SetFloat("Speed", moveSpeed);

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
            anim.SetTrigger("SpecialAttack");
        }

        Invoke("ResumeMovement", 1.5f);
    }

    void ResumeMovement()
    {
    }
}