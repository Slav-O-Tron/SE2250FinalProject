using UnityEngine;

public class SpiderMotion : EnemyMotion
{
    public float pounce = 10f;
    public float climbSpeed = 5f;
    public float rotationSpeed = 5f;
    public LayerMask walkableLayers;

    
    protected Animator anim;

    protected override void Start()
    {
        base.Start();
        agent.speed = 8f;
        agent.updateRotation = false;
        anim = GetComponent<Animator>();
    }

    protected override void Update()
    {
        RaycastHit hit;
        // Use -transform.forward because the model is imported facing backwards
        Vector3 rayDir = (-transform.forward * 0.5f) + (-transform.up);
        float currentMovingSpeed = 0f;

        if (Physics.Raycast(transform.position, rayDir, out hit, 3f, walkableLayers))
        {
            if (agent.enabled) agent.enabled = false;
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            // Move in the visual forward direction (negated because model faces backwards)
            transform.Translate(-Vector3.forward * climbSpeed * Time.deltaTime);

            currentMovingSpeed = climbSpeed;
        }
        else
        {
            if (!agent.enabled)
            {
                if (Physics.Raycast(transform.position, -Vector3.up, 1f, walkableLayers))
                    agent.enabled = true;
            }
            if (agent.enabled)
            {
                base.Update();

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

        if (anim != null)
            anim.SetFloat("Speed", currentMovingSpeed);
    }
}