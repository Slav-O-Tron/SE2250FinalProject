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
    [SerializeField] private string npcName = "Elder";

    [Header("References")]
    [SerializeField] private DialogueUI dialogueUI;

    [Header("Story Gift (optional)")]
    [Tooltip("Item to give the player after the opening story. Leave empty for levels without a gift.")]
    [SerializeField] private ItemData storyGiftItem;
    [Tooltip("Line the Elder says when handing over the gift.")]
    [SerializeField] private string storyGiftLine = "Take this — you will need it to fight off the enemies.";

    [Header("Chrono Shards")]
    [Tooltip("Optional inventory item used for the shard hand-in. If left empty, one is created at runtime.")]
    [SerializeField] private ItemData chronoShardItem;

    /// <summary>Fired once when the player finishes the opening story dialogue.</summary>
    public event Action OnStoryFinished;
    public event Action<int> OnChronoShardDelivered;

    private HUD hud;
    private LevelCompletion levelCompletion;
    private InventoryManager inventoryManager;
    private ItemData runtimeChronoShardItem;

    private bool playerInRange = false;
    private bool dialogueOpen = false;
    private bool hasSpokenBefore = false;
    private bool rewardReady = false;
    private bool rewardClaimed = false;
    private bool shardTurnInReady = false;
    private bool finalShardTurnIn = false;
    private int pendingShardWave = -1;

    // Dialogue 

    private DialogueLine[] StoryLines
    {
        get
        {
            var lines = new System.Collections.Generic.List<DialogueLine>
            {
                new DialogueLine { speakerName = npcName, text = "You actually came... I wasn't sure anyone still would." },
                new DialogueLine { speakerName = npcName, text = "My name is " + npcName + ". I have guarded the Chronosphere for thirty years." },
                new DialogueLine { speakerName = npcName, text = "When the eclipse came, the boundary between times shattered. The dead began walking again — pulled forward from history." },
                new DialogueLine { speakerName = npcName, text = "I can seal this realm, but the ritual needs Chrono Shards torn loose from the undead as they fall." },
                new DialogueLine { speakerName = npcName, text = "I need you to protect me, break each wave, and bring me one recovered shard after every assault." },
                new DialogueLine { speakerName = npcName, text = "When the final shard is in my hands, I can restore this realm's Chronosphere piece. Can you do that?" },
            };

            if (storyGiftItem != null)
                lines.Add(new DialogueLine { speakerName = npcName, text = storyGiftLine });

            return lines.ToArray();
        }
    }

    private DialogueLine[] ReminderLines => new DialogueLine[]
    {
        new DialogueLine
        {
            speakerName = npcName,
            text = "Hold the line. Recover the shard from the next wave and bring it back to me."
        },
    };

    private DialogueLine[] RewardLines => new DialogueLine[]
    {
        new DialogueLine
        {
            speakerName = npcName,
            text = "You did it. With those shards, the ritual is complete and the dead are falling back."
        },
        new DialogueLine
        {
            speakerName = npcName,
            text = "Take this fragment of the Chronosphere. It is one of five pieces needed to restore it."
        },
        new DialogueLine
        {
            speakerName = npcName,
            text = "And take these 100 coins — you earned them."
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
        inventoryManager = FindFirstObjectByType<InventoryManager>();

        if (dialogueUI == null)
            dialogueUI = FindFirstObjectByType<DialogueUI>();

        hud?.SetDefaultPrompt($"Speak with {npcName}");
    }

    private void Update()
    {
        if (!playerInRange || !Input.GetKeyDown(KeyCode.E) || dialogueOpen)
            return;

        if (shardTurnInReady)
        {
            TryDeliverChronoShard();
            return;
        }

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

        bool isRewardConversation = rewardReady && !rewardClaimed;
        if (isRewardConversation && levelCompletion != null && levelCompletion.TryOpenRewardPuzzle(OpenRewardDialogue, OnRewardPuzzleClosed))
        {
            dialogueOpen = true;
            hud?.HideInteractPrompt();
            if (hud != null) hud.ShowHUD(false);
            return;
        }

        dialogueOpen = true;
        hud?.HideInteractPrompt();
        if (hud != null) hud.ShowHUD(false);

        bool isFirstTime = !hasSpokenBefore;
        DialogueLine[] lines = GetCurrentDialogueLines();
        hasSpokenBefore = true;

        dialogueUI.StartDialogue(lines, onFinished: () =>
        {
            dialogueOpen = false;
            if (hud != null) hud.ShowHUD(true);

            // Notify WaveManager only after the opening story is done
            if (isFirstTime)
            {
                hud?.SetDefaultPrompt("Defend the Elder and recover the next Chrono Shard.");
                OnStoryFinished?.Invoke();
            }

            if (isRewardConversation)
            {
                rewardClaimed = levelCompletion != null && levelCompletion.ClaimChronosphereReward();
                if (rewardClaimed)
                {
                    Player player = FindFirstObjectByType<Player>();
                    player?.AddMoney(100);
                }
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

    private void OpenRewardDialogue()
    {
        dialogueOpen = true;
        hud?.HideInteractPrompt();
        if (hud != null) hud.ShowHUD(false);

        dialogueUI.StartDialogue(RewardLines, onFinished: () =>
        {
            dialogueOpen = false;
            if (hud != null) hud.ShowHUD(true);

            rewardClaimed = levelCompletion != null && levelCompletion.ClaimChronosphereReward();
            if (rewardClaimed)
            {
                Player player = FindFirstObjectByType<Player>();
                player?.AddMoney(100);
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

    private void OnRewardPuzzleClosed()
    {
        dialogueOpen = false;
        if (hud != null) hud.ShowHUD(true);

        if (playerInRange)
            hud?.ShowInteractPrompt(GetInteractPromptText());
        else
            hud?.HideInteractPrompt();
    }

    public void PrepareChronosphereReward()
    {
        rewardReady = true;
        hud?.SetDefaultPrompt("The ritual is complete. Speak with the Elder.");

        if (playerInRange)
            hud?.ShowInteractPrompt(GetInteractPromptText());
    }

    public void PrepareChronoShardTurnIn(int waveNumber, bool completesLevelAfterTurnIn)
    {
        if (inventoryManager == null)
            inventoryManager = FindFirstObjectByType<InventoryManager>();

        ItemData shardItem = GetChronoShardItem();
        if (inventoryManager != null && shardItem != null)
            inventoryManager.AddItem(shardItem, 1);

        shardTurnInReady = true;
        finalShardTurnIn = completesLevelAfterTurnIn;
        pendingShardWave = waveNumber;

        hud?.SetDefaultPrompt(completesLevelAfterTurnIn
            ? "Final shard recovered. Return it to the Elder."
            : "Wave cleared. Return the recovered Chrono Shard to the Elder.");

        if (playerInRange)
            hud?.ShowInteractPrompt(GetInteractPromptText());
    }

    public ItemData GetChronoShardItem()
    {
        if (chronoShardItem != null)
            return chronoShardItem;

        if (runtimeChronoShardItem == null)
        {
            runtimeChronoShardItem = ScriptableObject.CreateInstance<ItemData>();
            runtimeChronoShardItem.itemName = "Chrono Shard";
            runtimeChronoShardItem.description = "A shard pulled from the undead. The Elder needs it to stabilize the ritual.";
            runtimeChronoShardItem.isStackable = true;
        }

        return runtimeChronoShardItem;
    }

    private void TryDeliverChronoShard()
    {
        if (inventoryManager == null)
            inventoryManager = FindFirstObjectByType<InventoryManager>();

        ItemData shardItem = GetChronoShardItem();
        if (inventoryManager == null || shardItem == null)
        {
            Debug.LogWarning("[StoryNPC] InventoryManager or Chrono Shard item missing.");
            return;
        }

        if (!inventoryManager.HasItem(shardItem))
        {
            hud?.SetDefaultPrompt("Recover the Chrono Shard and return it to the Elder.");
            if (playerInRange)
                hud?.ShowInteractPrompt(GetInteractPromptText());
            return;
        }

        inventoryManager.RemoveItem(shardItem, 1);

        int deliveredWave = pendingShardWave;
        bool completesLevel = finalShardTurnIn;

        shardTurnInReady = false;
        finalShardTurnIn = false;
        pendingShardWave = -1;

        if (playerInRange)
            hud?.HideInteractPrompt();

        if (completesLevel)
            levelCompletion?.CompleteLevel();
        else
            hud?.SetDefaultPrompt("Shard delivered. Defend the Elder and prepare for the next wave.");

        OnChronoShardDelivered?.Invoke(deliveredWave);
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
        if (shardTurnInReady)
            return "Press E to give shard to the Elder";

        if (rewardReady && !rewardClaimed && levelCompletion != null && levelCompletion.HasPendingRewardPuzzle)
            return "Press E to arrange the 3 shards";

        if (rewardReady && !rewardClaimed)
            return "Press E to receive the restored Chronosphere piece";

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
