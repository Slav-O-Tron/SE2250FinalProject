using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInventory))]
public class Player : Entity
{
    public static Player ActivePlayer { get; private set; }

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float sprintSpeed = 8f;
    public float staminaDrainRate = 20f;
    public float gravity = 20f;
    public float jumpHeight = 2f;

    [Header("Look")]
    public float mouseSensitivity = 200f;
    public Transform cameraTransform;
    public float maxLookAngle = 60f;

    [Header("Combat")]
    public int attackDamage = 10;
    public float damageReduction = 0f;

    [Header("References")]
    public Animator animator;

    private float xRotation = 0f;
    private CharacterController controller;
    private float verticalVelocity;
    
    //Skill Tree Abilities
    private PlayerAbilities abilities;
    private bool hasDoubleJumped = false;
    private bool jumpPressed = false;
    private bool dashPressed = false;
    private bool isDashing = false;


    private InventoryManager inventoryManager;
    private PlayerInventory playerInventory;

    protected override void Awake()
    {
        base.Awake();
        controller      = GetComponent<CharacterController>();
        playerInventory = GetComponent<PlayerInventory>();
        abilities = GetComponent<PlayerAbilities>();
        RegisterAsActivePlayer();

        PlayerData pd = PlayerData.GetOrCreate();

        // Restore persisted gold
        money = pd.gold;

        // Restore persisted health (savedHealth == -1 means first load — stay at full)
        if (pd.savedHealth > 0)
            currentHealth = Mathf.Min(pd.savedHealth, maxHealth);
    }

    private void OnEnable()
    {
        RegisterAsActivePlayer();
    }

    private void OnDestroy()
    {
        if (ActivePlayer == this)
        {
            PlayerData pd = PlayerData.GetOrCreate();
            pd.gold = money;
            pd.savedHealth = currentHealth;
            ActivePlayer = null;
        }
    }

    public override void AddMoney(int amount)
    {
        base.AddMoney(amount);
        PlayerData.GetOrCreate().gold = money;
    }

    public override bool SpendMoney(int amount)
    {
        bool spent = base.SpendMoney(amount);
        if (spent)
            PlayerData.GetOrCreate().gold = money;
        return spent;
    }

    private void Start()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        if (Input.inputString.Length > 0)
            Debug.Log("inputString: '" + Input.inputString + "'");
        if (inventoryManager != null && inventoryManager.MenuActivated)
            return;
        //Adding doublejump and dash compatibility
        jumpPressed = Input.inputString.Contains(" ");
        dashPressed = Input.inputString == "q";
        
        if (dashPressed && abilities != null && abilities.canDash)
        {
            Debug.Log("Dash fired! canDash: " + abilities.canDash);
            Vector3 dashDir = transform.forward;
            Debug.Log("Dash direction: " + dashDir);
            controller.Move(dashDir * 30f);
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
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void MovePlayer()
    {
        //Dash Functionality
        if (dashPressed && abilities != null && abilities.canDash && !isDashing)
            StartCoroutine(DashCoroutine());
        
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
            hasDoubleJumped = false;

            if (Input.GetKeyDown(KeyCode.Space))
                verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;

            if (jumpPressed && abilities != null && abilities.canDoubleJump && !hasDoubleJumped)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
                hasDoubleJumped = true;
            }
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
            animator.SetBool("Run", isSprinting && Input.GetKey(KeyCode.W));
            animator.SetBool("RunLeft", isSprinting && Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A));
            animator.SetBool("RunRight", isSprinting && Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D));
            animator.SetBool("Jump", Input.GetKey(KeyCode.Space));
            animator.SetBool("Attack", Input.GetMouseButton(1));
        }
    }

    public void GainXP(int amount)
    {
        playerInventory?.AddXP(amount);
    }

    public void GainCoins(int amount)
    {
        AddMoney(amount);
    }
    
    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        float dashDuration = 0.15f;
        float dashSpeed = 30f / dashDuration;
        float elapsed = 0f;
    
        Vector3 dashDir = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;

        while (elapsed < dashDuration)  
        {
            controller.Move(dashDir * dashSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
    }
    

    public override void TakeDamage(int amount)
    {
        int reduced = Mathf.RoundToInt(amount * (1f - Mathf.Clamp01(damageReduction)));
        base.TakeDamage(reduced);
    }

    protected override void OnDamageTaken(int amount)
    {
        // TODO: hit flash, sound
    }

    protected override void OnDeath()
    {
        Debug.Log("Player died.");
        LevelFailManager failManager = LevelFailManager.Instance != null
            ? LevelFailManager.Instance
            : FindFirstObjectByType<LevelFailManager>();
        if (failManager != null)
            failManager.TriggerFail("You have fallen!");
        else
            Debug.LogError("[Player] No LevelFailManager found in scene. Add a LevelFailManager GameObject to this level.");
    }

    public static Player ResolveActivePlayer()
    {
        if (IsUsablePlayer(ActivePlayer))
            return ActivePlayer;

        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        Player bestPlayer = null;

        foreach (Player candidate in players)
        {
            if (ShouldReplaceActivePlayer(candidate, bestPlayer))
                bestPlayer = candidate;
        }

        ActivePlayer = bestPlayer;
        return ActivePlayer;
    }

    private void RegisterAsActivePlayer()
    {
        if (ShouldReplaceActivePlayer(this, ActivePlayer))
            ActivePlayer = this;
    }

    private static bool ShouldReplaceActivePlayer(Player candidate, Player current)
    {
        if (!IsUsablePlayer(candidate))
            return false;

        if (!IsUsablePlayer(current))
            return true;

        bool candidateHasCamera = candidate.cameraTransform != null;
        bool currentHasCamera = current.cameraTransform != null;

        if (candidateHasCamera != currentHasCamera)
            return candidateHasCamera;

        return false;
    }

    private static bool IsUsablePlayer(Player candidate)
    {
        return candidate != null && candidate.gameObject.activeInHierarchy;
    }
}