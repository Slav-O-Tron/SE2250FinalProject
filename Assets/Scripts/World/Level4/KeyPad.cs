using TMPro;
using UnityEngine;

public class KeyPad : MonoBehaviour
{
    [SerializeField] private TMP_Text Ans;
    [SerializeField] private Door door;
    [SerializeField] private string Answer = "BOE";

    private bool isOpen = false;

    public void SetOpenState(bool state)
    {
        isOpen = state;

        if (!isOpen && Ans != null)
        {
            Ans.text = "";
        }
    }

    public void AddLetter(string letter)
    {
        Debug.Log("AddLetter called with: " + letter);

        if (!isOpen)
        {
            Debug.LogWarning("Keypad is not open, ignoring button press.");
            return;
        }

        if (Ans == null)
        {
            Debug.LogError("Ans text is NOT assigned in the Inspector.");
            return;
        }

        if (Ans.text == "CORRECT" || Ans.text == "INCORRECT")
            Ans.text = "";

        Ans.text += letter;

        Debug.Log("Current keypad text: " + Ans.text);
    }

    public void Clear()
    {
        if (Ans == null)
        {
            Debug.LogError("Ans text is NOT assigned in the Inspector.");
            return;
        }

        Ans.text = "";
    }

    public void Execute()
    {
        if (!isOpen)
        {
            Debug.LogWarning("Keypad is not open, ignoring execute.");
            return;
        }

        if (Ans == null)
        {
            Debug.LogError("Ans text is NOT assigned in the Inspector.");
            return;
        }

        Debug.Log("Entered code: " + Ans.text);
        Debug.Log("Expected code: " + Answer);

        if (string.Equals(Ans.text, Answer, System.StringComparison.OrdinalIgnoreCase))
        {
            Ans.text = "CORRECT";
            Debug.Log("Keypad: correct code");

            if (door != null)
            {
                door.OpenFromKeypad();
            }
            else
            {
                Debug.LogError("Door reference is NULL");
            }
        }
        else
        {
            Ans.text = "INCORRECT";
            Debug.Log("Keypad: wrong code");
        }
    }
}