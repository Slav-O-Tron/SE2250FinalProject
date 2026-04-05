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

    private bool playerInRange = false;
    private bool dialogueOpen = false;
    private bool hasSpokenBefore = false;

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

    // ── Unity lifecycle ───────────────────────────────────────────────────────

    private void Start()
    {
        hud = FindFirstObjectByType<HUD>();

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
        hud?.ShowInteractPrompt($"Press E to speak with {npcName}");
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
        DialogueLine[] lines = hasSpokenBefore ? ReminderLines : StoryLines;
        hasSpokenBefore = true;

        dialogueUI.StartDialogue(lines, onFinished: () =>
        {
            dialogueOpen = false;
            if (hud != null) hud.ShowHUD(true);

            if (playerInRange)
                hud?.ShowInteractPrompt($"Press E to speak with {npcName}");
            else
                hud?.HideInteractPrompt();

            // Notify WaveManager only after the opening story is done
            if (isFirstTime)
            {
                hud?.SetDefaultPrompt("Defend the Elder. Waves incoming.");
                OnStoryFinished?.Invoke();
            }
        });
    }

    // ── Entity ────────────────────────────────────────────────────────────────

    protected override void OnDamageTaken(int amount)
    {
        Debug.Log($"[{npcName}] Took {amount} damage. HP: {currentHealth}/{maxHealth}");
    }

    protected override void OnDeath()
    {
        Debug.Log($"[{npcName}] Has fallen.");
        // ProtectedNPC handles the fail state if you also attach that component,
        // or wire this up to your own fail logic here.
    }
}
