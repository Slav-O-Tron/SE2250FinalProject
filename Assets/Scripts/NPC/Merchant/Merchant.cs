using UnityEngine;

/// Attach to your Merchant NPC GameObject.
/// Add a Collider (set as Trigger) to define the interaction radius.
/// Assign all fields in the Inspector.
public class Merchant : Entity
{
    [SerializeField] private string merchantName = "Merchant";

    private GameObject interactPrompt;

    [Header("Dialogue")]
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private DialogueLine[] introLines;        // First-time story dialogue
    [SerializeField] private DialogueLine[] repeatLines;       // Shorter lines for repeat visits

    [Header("Shop")]
    [SerializeField] private ShopItem[] shopItems;
    [SerializeField] private ShopUI shopUI;

    private bool playerInRange = false;
    private bool shopOpen = false;
    private bool dialogueOpen = false;

    // Tracks which level the player had last time the merchant spoke,
    // so a fresh warning plays each time they return between levels.
    private int lastSpokenLevel = -1;

    private HUD hud;

    // Pre-level challenge warnings — one entry per level (index 0 = level 1, etc.)
    private static readonly DialogueLine[][] levelWarningLines =
    {
        // Level 1
        new DialogueLine[]
        {
            new DialogueLine { speakerName = "Merchant", text = "The dead have been rising since the eclipse..." },
            new DialogueLine { speakerName = "Merchant", text = "The Chronosphere ahead twists time itself. Don't let it disorient you." },
            new DialogueLine { speakerName = "Merchant", text = "Stock up before you go. Stay safe out there." }
        },
        // Level 2
        new DialogueLine[]
        {
            new DialogueLine { speakerName = "Merchant", text = "You made it back. Good." },
            new DialogueLine { speakerName = "Merchant", text = "The next Chronosphere is colder — the creatures inside move faster." },
            new DialogueLine { speakerName = "Merchant", text = "I'd grab a fire flask if I were you." }
        },
        // Level 3
        new DialogueLine[]
        {
            new DialogueLine { speakerName = "Merchant", text = "Halfway there. The hardest part is still ahead." },
            new DialogueLine { speakerName = "Merchant", text = "Level 3 sits at a rift in time — enemies will split and respawn until the boss falls." },
            new DialogueLine { speakerName = "Merchant", text = "Don't waste your potions on the small ones." }
        },
        // Level 4
        new DialogueLine[]
        {
            new DialogueLine { speakerName = "Merchant", text = "Only two left. I can see the strain in your eyes." },
            new DialogueLine { speakerName = "Merchant", text = "The fourth Chronosphere warps gravity. Watch your footing." },
            new DialogueLine { speakerName = "Merchant", text = "Whatever you need — now is the time to get it." }
        },
        // Level 5
        new DialogueLine[]
        {
            new DialogueLine { speakerName = "Merchant", text = "This is it. The final Chronosphere." },
            new DialogueLine { speakerName = "Merchant", text = "The thing inside... it remembers every hero who failed before you." },
            new DialogueLine { speakerName = "Merchant", text = "Come back alive. I'll have the finest goods waiting." }
        },
    };

    private void Start()
    {
        hud = FindFirstObjectByType<HUD>();
        if (hud != null) interactPrompt = hud.interactPrompt;
        if (interactPrompt != null) interactPrompt.SetActive(false);

        repeatLines = new DialogueLine[]
        {
            new DialogueLine { speakerName = "Merchant", text = "Need anything?" }
        };
    }

    protected override void OnDeath()
    {
        Debug.Log($"{merchantName} has been defeated!");
        Destroy(gameObject);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (shopOpen)
            {
                CloseShop();
            }
            else if (!dialogueOpen)
            {
                OpenDialogue();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactPrompt != null) interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactPrompt != null) interactPrompt.SetActive(false);
            if (shopOpen) CloseShop();
            if (dialogueOpen) dialogueUI?.CloseDialogue();
            dialogueOpen = false;
            if (hud != null) hud.ShowHUD(true);
        }
    }

    private void OpenDialogue()
    {
        if (dialogueUI == null) return;

        dialogueOpen = true;
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (hud != null) hud.ShowHUD(false);

        DialogueLine[] linesToShow = GetDialogueForCurrentLevel();

        dialogueUI.StartDialogue(linesToShow, onFinished: () =>
        {
            dialogueOpen = false;
            if (hud != null) hud.ShowHUD(true);
            if (interactPrompt != null) interactPrompt.SetActive(true);
        });
    }

    private DialogueLine[] GetDialogueForCurrentLevel()
    {
        int level = PlayerData.Instance != null ? PlayerData.Instance.currentLevel : 1;

        // Show the level-specific warning the first time per level; repeat lines after that.
        if (level != lastSpokenLevel)
        {
            lastSpokenLevel = level;
            int index = Mathf.Clamp(level - 1, 0, levelWarningLines.Length - 1);
            return levelWarningLines[index];
        }

        return repeatLines;
    }

    public void OpenShopFromDialogue()
    {
        dialogueUI?.CloseDialogue();
        dialogueOpen = false;
        OpenShop();
    }

    private void OpenShop()
    {
        if (shopUI == null) return;
        shopOpen = true;
        if (hud != null) hud.ShowHUD(false);
        shopUI.OpenShop(shopItems);
        
    }

    private void CloseShop()
    {
        if (shopUI == null) return;
        shopOpen = false;
        if (hud != null) hud.ShowHUD(true);
        shopUI.CloseShop();
    }
}
