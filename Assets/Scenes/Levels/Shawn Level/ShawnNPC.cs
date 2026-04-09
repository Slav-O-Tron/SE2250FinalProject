using UnityEngine;
using System;

public class ShawnNPC : Entity
{
    [Header("Identity")]
    [SerializeField] private string npcName = "Ancient Spirit";

    [Header("References")]
    [SerializeField] private DialogueUI dialogueUI;

    public event Action OnStoryFinished;

    private HUD hud;
    private LevelCompletion levelCompletion;

    private bool playerInRange = false;
    private bool dialogueOpen = false;
    private bool hasSpokenBefore = true; // Skip intro
    private bool rewardReady = false;
    private bool rewardClaimed = false;

    private DialogueLine[] RewardLines => new DialogueLine[]
    {
        new DialogueLine
        {
            speakerName = npcName,
            text = "You have proven yourself worthy in combat."
        },
        new DialogueLine
        {
            speakerName = npcName,
            text = "Take this fragment of the Chronosphere. It is one of five pieces needed to restore it."
        },
        new DialogueLine
        {
            speakerName = npcName,
            text = "Carry it to the boat. Your path forward is open now."
        },
    };

    private DialogueLine[] ReminderLines => new DialogueLine[]
    {
        new DialogueLine
        {
            speakerName = npcName,
            text = "Go into the forest and defeat the enemies at the tower"
        },
    };

    private DialogueLine[] PostRewardLines => new DialogueLine[]
    {
        new DialogueLine
        {
            speakerName = npcName,
            text = "The piece is yours now. Go."
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
        hud?.SetDefaultPrompt("Speak with the Ancient Spirit to claim the Chronosphere piece.");

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

    private bool isInvincible = true;

    protected override void OnDamageTaken(int amount)
    {
        if (isInvincible)
        {
            // Heal back to full immediately
            Heal(amount);
        }
    }

    protected override void OnDeath()
    {
        Debug.Log($"[{npcName}] Has fallen.");
        LevelFailManager failManager = LevelFailManager.Instance != null
            ? LevelFailManager.Instance
            : FindFirstObjectByType<LevelFailManager>();
        if (failManager != null)
            failManager.TriggerFail("The Ancient Spirit has fallen!");
        else
            Debug.LogError("[ForestNPC] No LevelFailManager found in scene.");
    }
}