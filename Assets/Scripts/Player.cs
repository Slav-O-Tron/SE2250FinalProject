using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float sprintSpeed = 8f;
    public float gravity = 20f;
    public float jumpHeight = 2f;
    public float mouseSensitivity = 200f;
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

            if (Input.GetKeyDown(KeyCode.Space))
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        move.y = verticalVelocity;
        controller.Move(move * Time.deltaTime);

        Vector3 flatMove = new Vector3(horizontal, 0f, vertical);

        if (animator != null)
        {
            animator.SetFloat("Speed", flatMove.magnitude);
        }
    }
}