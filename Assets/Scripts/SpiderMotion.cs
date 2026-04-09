using UnityEngine;

public class SpiderMotion : MonoBehaviour
{
    public Transform target;
    private Animator anim;
    private float moveSpeed = 6f;

    void Start()
    {
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
        if (target == null || anim == null) return;

        // Move toward player
        Vector3 directionToPlayer = (target.position - transform.position).normalized;
        transform.position += directionToPlayer * moveSpeed * Time.deltaTime;

        // Set animation
        anim.SetFloat("Speed", moveSpeed);
    }
}