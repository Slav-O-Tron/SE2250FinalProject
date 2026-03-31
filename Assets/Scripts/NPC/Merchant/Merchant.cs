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
    private bool hasSpokenBefore = false;

    private HUD hud;

    private void Start()
    {
        hud = FindFirstObjectByType<HUD>();
        if (hud != null) interactPrompt = hud.interactPrompt;
        if (interactPrompt != null) interactPrompt.SetActive(false);

        introLines = new DialogueLine[]
        {
            new DialogueLine { speakerName = "Merchant", text = "The dead have been rising since the eclipse..." },
            new DialogueLine { speakerName = "Merchant", text = "I've been selling supplies to survivors ever since." },
            new DialogueLine { speakerName = "Merchant", text = "Take a look at what I have. Stay safe out there." }
        };

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

        DialogueLine[] linesToShow = (!hasSpokenBefore && introLines.Length > 0) ? introLines : repeatLines;
        hasSpokenBefore = true;

        dialogueUI.StartDialogue(linesToShow, onFinished: () =>
        {
            dialogueOpen = false;
            if (hud != null) hud.ShowHUD(true);
            if (interactPrompt != null) interactPrompt.SetActive(true);
        });
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
