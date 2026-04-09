using UnityEngine;

public class SpiderMotion : MonoBehaviour
{
    public Transform target; // ADD THIS BACK!
    private Animator anim;
    private float moveSpeed = 6f;
    private float rotationSpeed = 5f;

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

        Vector3 directionToPlayer = (target.position - transform.position).normalized;
        transform.position += directionToPlayer * moveSpeed * Time.deltaTime;

        if (directionToPlayer.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        anim.SetFloat("Speed", moveSpeed);
    }
}