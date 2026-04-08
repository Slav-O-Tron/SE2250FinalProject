using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShardAssemblyPuzzlePanel : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private TMP_Text currentOrderText;
    [SerializeField] private TMP_Text feedbackText;

    [Header("Buttons")]
    [SerializeField] private Button[] shardButtons;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button clearButton;
    [SerializeField] private Button closeButton;

    [Header("Puzzle")]
    [SerializeField] private string[] shardLabels = { "Dawn", "Dusk", "Void" };
    [SerializeField] private int[] correctOrder = { 0, 1, 2 };

    private readonly System.Collections.Generic.List<int> currentOrder = new System.Collections.Generic.List<int>();
    private InventoryManager inventoryManager;
    private Action onSolved;
    private Action onClosed;

    private void Awake()
    {
        if (panelRoot == null)
            panelRoot = gameObject;

        inventoryManager = FindFirstObjectByType<InventoryManager>();

        if (panelRoot != null)
            panelRoot.SetActive(false);

        WireButtons();
        ApplyButtonLabels();
    }

    public void Show(Action solvedCallback, Action closedCallback = null)
    {
        onSolved = solvedCallback;
        onClosed = closedCallback;

        currentOrder.Clear();
        RefreshText();

        if (titleText != null)
            titleText.text = "Arrange the 3 shards in the right order";

        if (instructionText != null)
            instructionText.text = "Choose all 3 shards in order. After each guess, the panel will tell you how many are in the correct position.";

        if (feedbackText != null)
            feedbackText.text = string.Empty;

        if (panelRoot != null)
            panelRoot.SetActive(true);

        SetMenuState(true);
    }

    private void Hide(bool notifyClosed)
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        SetMenuState(false);

        Action closedCallback = onClosed;
        onSolved = null;
        onClosed = null;

        if (notifyClosed)
            closedCallback?.Invoke();
    }

    private void WireButtons()
    {
        if (shardButtons != null)
        {
            for (int i = 0; i < shardButtons.Length; i++)
            {
                if (shardButtons[i] == null)
                    continue;

                int shardIndex = i;
                shardButtons[i].onClick.RemoveAllListeners();
                shardButtons[i].onClick.AddListener(() => SelectShard(shardIndex));
            }
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(ConfirmOrder);
        }

        if (clearButton != null)
        {
            clearButton.onClick.RemoveAllListeners();
            clearButton.onClick.AddListener(ClearOrder);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => Hide(true));
        }
    }

    private void ApplyButtonLabels()
    {
        if (shardButtons == null)
            return;

        for (int i = 0; i < shardButtons.Length; i++)
        {
            if (shardButtons[i] == null)
                continue;

            TMP_Text label = shardButtons[i].GetComponentInChildren<TMP_Text>(true);
            if (label != null)
                label.text = GetShardLabel(i);
        }
    }

    private void SelectShard(int shardIndex)
    {
        if (currentOrder.Count >= correctOrder.Length)
        {
            if (feedbackText != null)
                feedbackText.text = "Press Confirm or Clear before selecting again.";
            return;
        }

        if (currentOrder.Contains(shardIndex))
        {
            if (feedbackText != null)
                feedbackText.text = "You already placed that shard in this guess.";
            return;
        }

        currentOrder.Add(shardIndex);
        RefreshText();

        if (feedbackText != null)
            feedbackText.text = string.Empty;
    }

    private void ConfirmOrder()
    {
        if (currentOrder.Count != correctOrder.Length)
        {
            if (feedbackText != null)
                feedbackText.text = "Arrange all three shards before confirming.";
            return;
        }

        int correctPositions = 0;

        for (int i = 0; i < correctOrder.Length; i++)
        {
            if (currentOrder[i] == correctOrder[i])
                correctPositions++;
        }

        if (correctPositions < correctOrder.Length)
        {
            if (feedbackText != null)
                feedbackText.text = correctPositions + " of " + correctOrder.Length + " shards are in the correct position.";

            currentOrder.Clear();
            RefreshText();
            return;
        }

        if (feedbackText != null)
            feedbackText.text = "The shards lock into place.";

        Action solvedCallback = onSolved;
        Hide(false);
        solvedCallback?.Invoke();
    }

    private void ClearOrder()
    {
        currentOrder.Clear();
        RefreshText();

        if (feedbackText != null)
            feedbackText.text = string.Empty;
    }

    private void RefreshText()
    {
        if (currentOrderText == null)
            return;

        if (currentOrder.Count == 0)
        {
            currentOrderText.text = "Current Order: -";
            return;
        }

        StringBuilder builder = new StringBuilder("Current Order: ");
        for (int i = 0; i < currentOrder.Count; i++)
        {
            if (i > 0)
                builder.Append(" -> ");

            builder.Append(GetShardLabel(currentOrder[i]));
        }

        currentOrderText.text = builder.ToString();
    }

    private string GetShardLabel(int index)
    {
        if (shardLabels != null && index >= 0 && index < shardLabels.Length && !string.IsNullOrEmpty(shardLabels[index]))
            return shardLabels[index];

        return "Shard " + (index + 1);
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