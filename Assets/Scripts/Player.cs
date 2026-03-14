using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float sprintSpeed = 8f;
    public float gravity = 20f;
    public float jumpHeight = 2f;
    public float mouseSensitivity = 200f;
    public float jumpAnimationCooldown = 4f;
    private float lastJumpAnimationTime = -999f;
    public Animator animator;

    private CharacterController controller;
    private float verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        LookAround();
        MovePlayer();
    }

    private void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(0f, mouseX, 0f);
    }

    private void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        move *= currentSpeed;
if (controller.isGrounded)
{
    verticalVelocity = -2f;

    if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastJumpAnimationTime + jumpAnimationCooldown)
    {
        verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
        lastJumpAnimationTime = Time.time;

        if (animator != null)
        {
            animator.SetTrigger("Jump");
        }
    }
}
else
{
    verticalVelocity -= gravity * Time.deltaTime;
}



        move.y = verticalVelocity;
        controller.Move(move * Time.deltaTime);


        if (animator != null)
        {
            animator.SetBool("isWalking", Input.GetKey(KeyCode.W));
            animator.SetBool("isWalkingBack", Input.GetKey(KeyCode.S));
            animator.SetBool("TurnLeft", Input.GetKey(KeyCode.A));
            animator.SetBool("TurnRight", Input.GetKey(KeyCode.D));
            animator.SetBool("WalkRight", Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D));
            animator.SetBool("WalkLeft", Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A));
            animator.SetBool("BackRight", Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D));
            animator.SetBool("BackLeft", Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A));
            animator.SetBool("Run", Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift));
            animator.SetBool("RunLeft", Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.A));
            animator.SetBool("RunRight", Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.D));
            animator.SetBool("Jump", Input.GetKey(KeyCode.Space));

        }

        }
}
    
