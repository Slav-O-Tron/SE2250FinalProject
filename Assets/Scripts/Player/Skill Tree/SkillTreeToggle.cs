using UnityEngine;

public class SkillTreeToggle : MonoBehaviour
{
    public GameObject skillTreePanel;


    void Start()
    {
        skillTreePanel.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            Toggle();
    }

    void Toggle()
    {
        bool isOpen = skillTreePanel.activeSelf;
        skillTreePanel.SetActive(!isOpen);

        Cursor.lockState = isOpen ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible   = !isOpen;
    }
}