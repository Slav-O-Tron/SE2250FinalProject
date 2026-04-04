using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
    }

    public void Play()
    {
        // Reset progression so a fresh run always starts at level 1.
        if (PlayerData.Instance != null)
            PlayerData.Instance.ResetProgress();

        SceneManager.LoadScene("MainWorld");
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
