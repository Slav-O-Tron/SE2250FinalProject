using UnityEngine;
using System;

public class DungeonNPC : Entity
{
    [Header("Identity")]
    [SerializeField] private string npcName = "Prisoner";

    [Header("References")]
    [SerializeField] private DialogueUI dialogueUI;

    public event Action OnStoryFinished;

    private HUD hud;
    private LevelCompletion levelCompletion;
    private Player playerController;

    private bool playerInRange = false;
    private bool dialogueOpen = false;
    private bool rewardReady = false;
    private bool rewardClaimed = false;
    private bool hasTriggeredCompletion = false;

    private DialogueLine[] RewardLines => new DialogueLine[]
    {
        new DialogueLine { speakerName = npcName, text = "Thank you for rescuing me." },
        new DialogueLine { speakerName = npcName, text = "Here is the Chrono Crystal you came here for." },
    };

    private void Start()
    {
        hud = FindFirstObjectByType<HUD>();
        levelCompletion = FindFirstObjectByType<LevelCompletion>();
        playerController = FindFirstObjectByType<Player>();

        if (dialogueUI == null)
            dialogueUI = FindFirstObjectByType<DialogueUI>();

        hud?.SetDefaultPrompt($"Speak with {npcName}");
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !dialogueOpen)
            OpenDialogue();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        hud?.ShowInteractPrompt($"Press E to speak with {npcName}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        hud?.HideInteractPrompt();

        if (dialogueOpen)
            dialogueUI?.CloseDialogue();

        dialogueOpen = false;

        if (playerController != null)
        {
            playerController.movementLocked = false;
            playerController.ResetVelocity();
        }

        if (hud != null)
            hud.ShowHUD(true);
    }

    private void OpenDialogue()
    {
        if (dialogueUI == null) return;

        dialogueOpen = true;
        hud?.HideInteractPrompt();
        if (hud != null) hud.ShowHUD(false);

        if (playerController != null)
            playerController.movementLocked = true;

        if (!hasTriggeredCompletion)
        {
            hasTriggeredCompletion = true;
            if (levelCompletion != null)
                levelCompletion.CompleteLevel();
        }

        dialogueUI.StartDialogue(RewardLines, onFinished: () =>
        {
            dialogueOpen = false;

            if (playerController != null)
            {
                playerController.movementLocked = false;
                playerController.ResetVelocity();
            }

            if (hud != null) hud.ShowHUD(true);

            rewardClaimed = levelCompletion != null && levelCompletion.ClaimChronosphereReward();

            hud?.HideInteractPrompt();
        });
    }

    public void PrepareChronosphereReward() { }

    protected override void OnDamageTaken(int amount)
    {
        Debug.Log($"[{npcName}] Took {amount} damage. HP: {currentHealth}/{maxHealth}");
    }

    protected override void OnDeath()
    {
        Debug.Log($"[{npcName}] Has fallen.");
        LevelFailManager failManager = LevelFailManager.Instance != null
            ? LevelFailManager.Instance
            : FindFirstObjectByType<LevelFailManager>();

        if (failManager != null)
            failManager.TriggerFail("The Prisoner has fallen!");
        else
            Debug.LogError("[DungeonNPC] No LevelFailManager found in scene.");
    }
}