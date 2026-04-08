using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;   // Drag DialoguePanel here
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueBodyText;
    [SerializeField] private Button nextButton;
    [SerializeField] private TMP_Text nextButtonText;
    [SerializeField] public Button shopButton;           // Shows "Open Shop" at the end

    private DialogueLine[] lines;
    private int currentLine = 0;
    private Action onDialogueFinished;
    private Action onShopSelected;
    private InventoryManager inventoryManager;

    private void Awake()
    {
        AutoBindReferences();
    }

    private void Start()
    {
        AutoBindReferences();

        if (nextButton == null)
        {
            Debug.LogError("[DialogueUI] Next Button is not assigned and could not be auto-found.");
            return;
        }

        nextButton.onClick.AddListener(OnNextPressed);
        if (shopButton != null) shopButton.gameObject.SetActive(false);
    }

    /// Start a dialogue sequence. onFinished is called when the player closes it.
    public void StartDialogue(DialogueLine[] dialogueLines, Action onFinished = null)
    {
        StartDialogue(dialogueLines, onFinished, null);
    }

    public void StartDialogue(DialogueLine[] dialogueLines, Action onFinished, Action onShopPressed)
    {
        AutoBindReferences();

        if (dialoguePanel == null || speakerNameText == null || dialogueBodyText == null || nextButton == null)
        {
            Debug.LogError("[DialogueUI] Missing required UI references. Check DialoguePanel, SpeakerNameText, DialogueBodyText, and NextButton.");
            return;
        }

        lines = dialogueLines;
        currentLine = 0;
        onDialogueFinished = onFinished;
        onShopSelected = onShopPressed;

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(OnNextPressed);

        if (shopButton != null) shopButton.gameObject.SetActive(false);

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        SetMenuState(true);

        ShowLine(currentLine);
    }

    public void CloseDialogue()
    {
        CloseDialogue(true);
    }

    private void CloseDialogue(bool restoreGameplayState)
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        if (shopButton != null)
            shopButton.gameObject.SetActive(false);

        if (restoreGameplayState)
            SetMenuState(false);
    }

    private void OnNextPressed()
    {
        currentLine++;

        if (currentLine < lines.Length)
        {
            ShowLine(currentLine);
        }
        else
        {
            // Dialogue over — show shop button if assigned, then close
            if (shopButton != null && onShopSelected != null)
            {
                shopButton.gameObject.SetActive(true);
                nextButtonText.text = "Close";

                shopButton.onClick.RemoveAllListeners();
                shopButton.onClick.AddListener(() =>
                {
                    CloseDialogue(false);
                    onDialogueFinished?.Invoke();
                    onShopSelected?.Invoke();
                });

                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(() =>
                {
                    CloseDialogue();
                    onDialogueFinished?.Invoke();
                });
            }
            else
            {
                CloseDialogue();
                onDialogueFinished?.Invoke();
            }
        }
    }

    private void ShowLine(int index)
    {
        speakerNameText.text = lines[index].speakerName;
        dialogueBodyText.text = lines[index].text;

        bool isLast = index == lines.Length - 1;
        if (nextButtonText != null)
            nextButtonText.text = isLast ? "Close" : "Next";
    }

    private void AutoBindReferences()
    {
        if (inventoryManager == null)
            inventoryManager = FindFirstObjectByType<InventoryManager>();

        if (dialoguePanel == null)
            dialoguePanel = gameObject;

        if (nextButton == null)
            nextButton = GetComponentInChildren<Button>(true);

        if (speakerNameText == null || dialogueBodyText == null || nextButtonText == null)
        {
            TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
            foreach (TMP_Text text in texts)
            {
                string lowerName = text.gameObject.name.ToLowerInvariant();

                if (speakerNameText == null && lowerName.Contains("speaker"))
                    speakerNameText = text;
                else if (dialogueBodyText == null && (lowerName.Contains("body") || lowerName.Contains("dialogue")))
                    dialogueBodyText = text;
                else if (nextButtonText == null && lowerName.Contains("next"))
                    nextButtonText = text;
            }
        }

        if (nextButtonText == null && nextButton != null)
            nextButtonText = nextButton.GetComponentInChildren<TMP_Text>(true);
    }

    private void SetMenuState(bool isOpen)
    {
        if (inventoryManager != null)
        {
            inventoryManager.SetExternalMenuActive(isOpen);
            return;
        }

        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;
    }
}
