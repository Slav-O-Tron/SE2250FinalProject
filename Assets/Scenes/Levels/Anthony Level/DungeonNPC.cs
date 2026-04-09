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

    private bool playerInRange = false;
    private bool dialogueOpen = false;
    private bool rewardReady = false;
    private bool rewardClaimed = false;
    private bool hasTriggeredCompletion = false;

    private DialogueLine[] RewardLines => new DialogueLine[]
    {
        new DialogueLine
        {
            speakerName = npcName,
            text = "Thank you for rescuing me."
        },
        new DialogueLine
        {
            speakerName = npcName,
            text = "Here is the Chrono Crystal you came here for."
        },
    };

    private DialogueLine[] PostRewardLines => new DialogueLine[]
    {
        new DialogueLine
        {
            speakerName = npcName,
            text = "Now go, before more of them come."
        },
    };

    private DialogueLine[] ReminderLines => new DialogueLine[]
    {
        new DialogueLine
        {
            speakerName = npcName,
            text = "Please, get me out of here."
        },
    };

    private void Start()
    {
        hud = FindFirstObjectByType<HUD>();
        levelCompletion = FindFirstObjectByType<LevelCompletion>();

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
        hud?.ShowInteractPrompt(GetInteractPromptText());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        hud?.HideInteractPrompt();
        if (dialogueOpen) dialogueUI?.CloseDialogue();
        dialogueOpen = false;
        if (hud != null) hud.ShowHUD(true);
    }

    private void OpenDialogue()
    {
        if (dialogueUI == null) return;

        dialogueOpen = true;
        hud?.HideInteractPrompt();
        if (hud != null) hud.ShowHUD(false);

        if (!hasTriggeredCompletion)
        {
            hasTriggeredCompletion = true;
            if (levelCompletion != null)
                levelCompletion.CompleteLevel();
        }

        bool isRewardConversation = rewardReady && !rewardClaimed;
        DialogueLine[] lines = GetCurrentDialogueLines();

        dialogueUI.StartDialogue(lines, onFinished: () =>
        {
            dialogueOpen = false;
            if (hud != null) hud.ShowHUD(true);

            if (isRewardConversation)
                rewardClaimed = levelCompletion != null && levelCompletion.ClaimChronosphereReward();

            if (playerInRange)
            {
                if (rewardClaimed)
                    hud?.HideInteractPrompt();
                else
                    hud?.ShowInteractPrompt(GetInteractPromptText());
            }
            else
                hud?.HideInteractPrompt();
        });
    }

    public void PrepareChronosphereReward()
    {
        rewardReady = true;
        hud?.SetDefaultPrompt($"Speak with {npcName} to claim the Chronosphere piece.");

        if (playerInRange)
            hud?.ShowInteractPrompt(GetInteractPromptText());
    }

    private DialogueLine[] GetCurrentDialogueLines()
    {
        if (rewardReady && !rewardClaimed)
            return RewardLines;

        if (rewardClaimed)
            return PostRewardLines;

        return ReminderLines;
    }

    private string GetInteractPromptText()
    {
        if (rewardReady && !rewardClaimed)
            return "Press E to receive the Chronosphere piece";

        return $"Press E to speak with {npcName}";
    }

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