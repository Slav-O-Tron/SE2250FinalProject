using UnityEngine;
using TMPro;

/// <summary>
/// Displays a single objective line on screen (e.g. "Speak with the Elder to begin").
/// Any script can call ObjectiveUI.Set("text") to update it at any time.
///
/// Scene setup:
///   Create a Canvas (Screen Space - Overlay) → add a child TextMeshPro object.
///   Attach this script to that TextMeshPro object.
/// </summary>
public class ObjectiveUI : MonoBehaviour
{
    public static ObjectiveUI Instance { get; private set; }

    private TMP_Text label;

    private void Awake()
    {
        Instance = this;
        label = GetComponent<TMP_Text>();
    }

    /// <summary>Update the on-screen objective text.</summary>
    public static void Set(string message)
    {
        if (Instance == null || Instance.label == null) return;
        Instance.label.text = message;
        Instance.gameObject.SetActive(true);
    }

    /// <summary>Hide the objective text entirely.</summary>
    public static void Hide()
    {
        if (Instance != null)
            Instance.gameObject.SetActive(false);
    }
}
