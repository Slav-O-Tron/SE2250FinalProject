using TMPro;
using UnityEngine;

public class KeyPad : MonoBehaviour
{
    [SerializeField] private TMP_Text Ans;
    [SerializeField] private Door door;
    [SerializeField] private string Answer = "BOE";

    public void AddLetter(string letter)
    {
        Debug.Log("AddLetter called with: " + letter);

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
                Debug.Log("Keypad: door reference found");
                door.OpenFromKeypad();
            }
            else
            {
                Debug.LogError("Keypad: door reference is NULL");
            }
        }
        else
        {
            Ans.text = "INCORRECT";
            Debug.Log("Keypad: wrong code");
        }
    }
}