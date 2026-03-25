using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInventory))]
public class Player : Entity
{
    public float moveSpeed = 4f;
    public float sprintSpeed = 8f;
    public float staminaDrainRate = 20f;
    public float gravity = 20f;
    public float jumpHeight = 2f;
    public float mouseSensitivity = 200f;
    public Transform cameraTransform;
    public float maxLookAngle = 60f;
    public Animator animator;

    private float xRotation = 0f;
    private CharacterController controller;
    private float verticalVelocity;

    private InventoryManager inventoryManager;
    private PlayerInventory playerInventory;

    protected override void Awake()
    {
        base.Awake();
        controller      = GetComponent<CharacterController>();
        playerInventory = GetComponent<PlayerInventory>();
    }

    void Start()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        if (inventoryManager != null && inventoryManager.MenuActivated)
        {
            return;
        }

        LookAround();
        MovePlayer();
    }

    private void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(0f, mouseX, 0f);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    private void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical   = Input.GetAxis("Vertical");

        bool wantsSprint = Input.GetKey(KeyCode.LeftShift);
        bool isSprinting = wantsSprint && playerInventory != null && playerInventory.HasStamina();

        if (isSprinting)
            playerInventory.UseStamina(staminaDrainRate * Time.deltaTime);

        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
        Vector3 move = (transform.right * horizontal + transform.forward * vertical) * currentSpeed;

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
            animator.SetBool("Run",      isSprinting && Input.GetKey(KeyCode.W));
            animator.SetBool("RunLeft",  isSprinting && Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A));
            animator.SetBool("RunRight", isSprinting && Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D));
            animator.SetBool("Jump", Input.GetKey(KeyCode.Space));
        }
    }
    
    /// Award XP — call this from Zombie.OnDeath, quest completion, etc.
    public void GainXP(int amount)     => playerInventory?.AddXP(amount);
 
    /// <summary>Award coins — also called by CoinPickup.</summary>
    public void GainCoins(int amount)  => AddMoney(amount);   // AddMoney lives in Entity
 
    // Entity abstract implementation 
 
    protected override void OnDamageTaken(int amount)
    {
        // TODO: hit flash, sound, UI health bar update
    }
 
    protected override void OnDeath()
    {
        // TODO: trigger game over screen
        Debug.Log("Player died.");
    }
}