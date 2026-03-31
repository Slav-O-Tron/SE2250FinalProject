using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// Attach to your DialoguePanel UI GameObject.
/// Scene setup:
///   DialoguePanel
///   ├── SpeakerNameText  (TMP_Text)
///   ├── DialogueBodyText (TMP_Text)
///   ├── NextButton       (Button) - "Next" / "Close"
///   └── ShopButton       (Button) - shown only after dialogue ends (can be hidden initially)
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

    private void Start()
    {
        nextButton.onClick.AddListener(OnNextPressed);
        if (shopButton != null) shopButton.gameObject.SetActive(false);
    }

    /// <summary>Start a dialogue sequence. onFinished is called when the player closes it.</summary>
    public void StartDialogue(DialogueLine[] dialogueLines, Action onFinished = null)
    {
        lines = dialogueLines;
        currentLine = 0;
        onDialogueFinished = onFinished;

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(OnNextPressed);

        if (shopButton != null) shopButton.gameObject.SetActive(false);

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowLine(currentLine);
    }

    public void CloseDialogue()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
            if (shopButton != null)
            {
                shopButton.gameObject.SetActive(true);
                nextButtonText.text = "Close";
                // Clicking Next again just closes
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
            nextButtonText.text = isLast ? "Done" : "Next";
    }
}
