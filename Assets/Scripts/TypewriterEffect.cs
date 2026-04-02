using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TypewriterClean : MonoBehaviour
{
    public TMP_Text textComponent;
    [TextArea] public string fullText;
    public float characterDelay = 0.02f;

    private bool isTyping = true;
    private bool finished = false;

    void Start()
    {
        textComponent.text = fullText;
        textComponent.maxVisibleCharacters = 0;
        StartCoroutine(TypeText());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // Skip typing
                StopAllCoroutines();
                textComponent.maxVisibleCharacters = fullText.Length;
                isTyping = false;
                finished = true;
            }
            else if (finished)
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }

    IEnumerator TypeText()
    {
        int totalChars = fullText.Length;

        for (int i = 0; i <= totalChars; i++)
        {
            textComponent.maxVisibleCharacters = i;
            yield return new WaitForSeconds(characterDelay);
        }

        isTyping = false;
        finished = true;
    }
    
    public void Continue()
    {
        SceneManager.LoadScene("MainMenu");
    }
}