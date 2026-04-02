using UnityEngine;
using UnityEngine.SceneManagement;
    public class StartScreen : MonoBehaviour
    {
    public void Continue()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
