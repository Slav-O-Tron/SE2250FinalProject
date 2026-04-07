using UnityEngine;
using System;

/// Attach to your story NPC in the level scene.
/// Requires a Trigger Collider on the same GameObject.
/// Also requires a DialogueUI somewhere in the scene (same one used by Merchant is fine).
///
/// First conversation: full backstory lines, ending with the protect-me briefing.
/// WaveManager subscribes to OnStoryFinished and starts waves only after dialogue ends.
public class StoryNPC : Entity
{
    [Header("Identity")]
    [SerializeField] private string npcName = "Elder Varos";

    [Header("References")]
    [SerializeField] private DialogueUI dialogueUI;

    /// <summary>Fired once when the player finishes the opening story dialogue.</summary>
    public event Action OnStoryFinished;

    private HUD hud;
    private LevelCompletion levelCompletion;

    private bool playerInRange = false;
    private bool dialogueOpen = false;
    private bool hasSpokenBefore = false;
    private bool rewardReady = false;
    private bool rewardClaimed = false;

    // Dialogue 

    private DialogueLine[] StoryLines => new DialogueLine[]
    {
        new DialogueLine
        {
            speakerName = npcName,
            text = "You actually came... I wasn't sure anyone still would."
        },
        new DialogueLine
        {
            speakerName = npcName,
            text = "My name is " + npcName + ". I have guarded the Chronosphere for thirty years."
        },
        new DialogueLine
        {
            speakerName = npcName,
            text = "When the eclipse came, the boundary between times shattered. The dead began walking again — pulled forward from history."
        },
        new DialogueLine
        {
            speakerName = npcName,
            text = "I know how to seal it. But the ritual takes time, and they will come for me the moment I begin."
        },
        new DialogueLine
        {
            speakerName = npcName,
            text = "I need you to protect me while I work."
        },
        new DialogueLine
        {
            speakerName = npcName,
            text = "The dead will arrive in waves. Hold them off until the seal is complete. Can you do that?"
        },
    };

    private DialogueLine[] ReminderLines => new DialogueLine[]
    {
        new DialogueLine
        {
            speakerName = npcName,
            text = "Stay close. They will come again."
        },
    };

    private DialogueLine[] RewardLines => new DialogueLine[]
    {
        new DialogueLine
        {
            speakerName = npcName,
            text = "You did it. The seal held, and the dead are falling back."
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

    private DialogueLine[] PostRewardLines => new DialogueLine[]
    {
        new DialogueLine
        {
            speakerName = npcName,
            text = "The piece is yours now. Go."
        },
    };

    // ── Unity lifecycle ───────────────────────────────────────────────────────

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

    // ── Trigger ───────────────────────────────────────────────────────────────

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

    // ── Dialogue ──────────────────────────────────────────────────────────────

    private void OpenDialogue()
    {
        if (dialogueUI == null) return;

        dialogueOpen = true;
        hud?.HideInteractPrompt();
        if (hud != null) hud.ShowHUD(false);

        bool isFirstTime = !hasSpokenBefore;
        bool isRewardConversation = rewardReady && !rewardClaimed;
        DialogueLine[] lines = GetCurrentDialogueLines();
        hasSpokenBefore = true;

        dialogueUI.StartDialogue(lines, onFinished: () =>
        {
            dialogueOpen = false;
            if (hud != null) hud.ShowHUD(true);

            // Notify WaveManager only after the opening story is done
            if (isFirstTime)
            {
                hud?.SetDefaultPrompt("Defend the Elder. Waves incoming.");
                OnStoryFinished?.Invoke();
            }

            if (isRewardConversation)
            {
                rewardClaimed = levelCompletion != null && levelCompletion.ClaimChronosphereReward();
            }

            if (playerInRange)
            {
                if (rewardClaimed)
                    hud?.HideInteractPrompt();
                else
                    hud?.ShowInteractPrompt(GetInteractPromptText());
            }
            else
            {
                hud?.HideInteractPrompt();
            }
        });
    }

    public void PrepareChronosphereReward()
    {
        rewardReady = true;
        hud?.SetDefaultPrompt("Speak with the Elder to claim the Chronosphere piece.");

        if (playerInRange)
            hud?.ShowInteractPrompt(GetInteractPromptText());
    }

    private DialogueLine[] GetCurrentDialogueLines()
    {
        if (rewardReady && !rewardClaimed)
            return RewardLines;

        if (!hasSpokenBefore)
            return StoryLines;

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

    // ── Entity ────────────────────────────────────────────────────────────────

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
            failManager.TriggerFail("The Elder has fallen!");
        else
            Debug.LogError("[StoryNPC] No LevelFailManager found in scene.");
    }
}
