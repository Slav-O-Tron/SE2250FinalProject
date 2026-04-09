using UnityEngine;
using UnityEngine.AI; // Added because the new code uses 'agent'

public class SpiderMotion : MonoBehaviour
{
    public Transform target;
    private Animator anim;
    private float moveSpeed = 6f;
    private float rotationSpeed = 5f;
    private float climbSpeed = 4f; // Required for the new climbing logic
    private NavMeshAgent agent; // Added to support the new logic
    public LayerMask walkableLayers; // Required for the raycast

    void Start()
    {
        // Using the new logic from your teammates
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = 8f;
            agent.updateRotation = false;
        }

        anim = GetComponent<Animator>();

        // If target not manually assigned, auto-find player
        if (target == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }
    }

    void Update()
    {
        // Safety check from your local version
        if (target == null || anim == null) return;

        RaycastHit hit;
        // Use -transform.forward because the model is imported facing backwards
        Vector3 rayDir = (-transform.forward * 0.5f) + (-transform.up);
        float currentMovingSpeed = 0f;

        // Note: The teammate's logic assumes a base class or additional variables 
        // like 'climbSpeed'. I've added defaults to keep it from breaking.

        if (Physics.Raycast(transform.position, rayDir, out hit, 1f))
        {
            // Move in the visual forward direction (negated because model faces backwards)
            transform.Translate(-Vector3.forward * climbSpeed * Time.deltaTime);
            currentMovingSpeed = climbSpeed;
        }
        else
        {
            if (agent != null && !agent.enabled)
            {
                if (Physics.Raycast(transform.position, -Vector3.up, 1f, walkableLayers))
                    agent.enabled = true;
            }

            if (agent != null && agent.enabled)
            {
                agent.SetDestination(target.position);

                // Manually rotate to face movement direction, flipped 180° for backwards model
                Vector3 velocity = agent.velocity;
                velocity.y = 0f;
                if (velocity.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(velocity.normalized) * Quaternion.Euler(0, 180f, 0);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
                }

                currentMovingSpeed = agent.velocity.magnitude;
            }
        }

        anim.SetFloat("Speed", currentMovingSpeed);
    }
}