using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

/// Attach one instance of this to any persistent GameObject (e.g. the Core
/// or a dedicated GameEnding object) in every level scene AND in MainWorld.
///
/// The moment PlayerData reports all 5 Chronosphere pieces collected,
/// this script fades to black and loads the end scene.
public class GameEnding : MonoBehaviour
{
    [Tooltip("Exact name of the end/credits scene as it appears in Build Settings.")]
    [SerializeField] private string endSceneName = "EndScene";

    [Tooltip("How many seconds to wait before loading the end scene.")]
    [SerializeField] private float delayBeforeLoad = 3f;

    [Tooltip("Optional full-screen black panel to fade in. Leave empty to skip fade.")]
    [SerializeField] private CanvasGroup fadePanel;

    [Tooltip("Optional message shown on screen before the transition.")]
    [SerializeField] private TextMeshProUGUI endingMessageText;

    [Tooltip("Message to display when the Chronosphere is restored.")]
    [SerializeField] private string endingMessage = "The Chronosphere is restored.\nTime flows once more.";

    private bool triggered = false;

    private void Update()
    {
        if (triggered) return;

        PlayerData pd = PlayerData.GetOrCreate();
        if (pd.HasCompletedChronosphere())
            TriggerEnding();
    }

    /// Call this directly if you want to trigger the ending from another script.
    public void TriggerEnding()
    {
        if (triggered) return;
        triggered = true;

        Debug.Log("[GameEnding] All 5 Chronosphere pieces collected — triggering ending.");
        StartCoroutine(EndingSequence());
    }

    private IEnumerator EndingSequence()
    {
        // Show the ending message
        if (endingMessageText != null)
        {
            endingMessageText.gameObject.SetActive(true);
            endingMessageText.text = endingMessage;
        }

        // Unlock and show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Fade to black if a panel is assigned
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            float elapsed = 0f;
            float fadeDuration = Mathf.Min(delayBeforeLoad, 2f);

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadePanel.alpha = Mathf.Clamp01(elapsed / fadeDuration);
                yield return null;
            }
            fadePanel.alpha = 1f;
        }
        else
        {
            yield return new WaitForSeconds(delayBeforeLoad);
        }

        SceneManager.LoadScene(endSceneName);
    }
}
