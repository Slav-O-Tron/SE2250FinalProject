using TMPro;
using UnityEngine;

public class KeyPad : MonoBehaviour
{
    [SerializeField] private TMP_Text Ans;
    [SerializeField] private Door door;

    [SerializeField] private string Answer = "BOE";

    public void AddLetter(string letter)
    {
        if (Ans.text == "CORRECT" || Ans.text == "INCORRECT")
            Ans.text = "";

        Ans.text += letter;
    }

    public void Clear()
    {
        Ans.text = "";
    }

    public void Execute()
    {
        if (string.Equals(Ans.text, Answer, System.StringComparison.OrdinalIgnoreCase))
        {
            Ans.text = "CORRECT";

            if (door != null)
                door.OpenFromKeypad();
        }
        else
        {
            Ans.text = "INCORRECT";
        }
    }
}