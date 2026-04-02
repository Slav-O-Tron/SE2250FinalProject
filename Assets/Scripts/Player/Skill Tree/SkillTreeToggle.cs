using UnityEngine;

public class SkillTreeToggle : MonoBehaviour
{
    private GameObject skillTreePanel;

    void Start()
    {
        Transform found = transform.Find("SkillTree");
        if (found != null)
            skillTreePanel = found.gameObject;

        if (skillTreePanel != null)
            skillTreePanel.SetActive(false);
        else
            Debug.LogWarning("SkillTreeToggle: could not find SkillTree panel as child.");
    }

    void Update()
    {
        if (Input.inputString == "k")
            Toggle();
    }

    void Toggle()
    {
        if (skillTreePanel == null) return;

        bool isOpen = skillTreePanel.activeSelf;
        skillTreePanel.SetActive(!isOpen);

        Cursor.lockState = isOpen ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isOpen;
    }
}