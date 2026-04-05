using UnityEngine;
using TMPro;

public class PuzzleButton : MonoBehaviour
{
    public int buttonID;
    public OrderPuzzle puzzleManager;
    public GameObject promptText;
    private bool playerNearby = false;
    private bool hasBeenPressed = false;
    private TextMeshProUGUI buttonText;

    void Start()
    {
        if (promptText != null)
        {
            promptText.SetActive(false);
            buttonText = promptText.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        if (playerNearby && !hasBeenPressed && Input.GetKeyDown(KeyCode.E))
            Press();
    }

    private void Press()
    {
        hasBeenPressed = true;
        puzzleManager.ButtonPressed(buttonID);
        transform.position += new Vector3(0, -0.1f, 0);
        if (buttonText != null)
            buttonText.text = "Pressed";
        Debug.Log("Button " + buttonID + " pressed");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.gameObject.name + " tag: " + other.tag);
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            if (promptText != null)
                promptText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (promptText != null)
                promptText.SetActive(false);
        }
    }
}