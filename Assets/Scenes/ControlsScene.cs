using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsScreen : MonoBehaviour
{
    public void GoToMainWorld()
    {
        SceneManager.LoadScene("MainWorld");
    }
}