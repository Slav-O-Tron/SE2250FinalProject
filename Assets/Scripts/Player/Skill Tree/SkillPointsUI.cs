using TMPro;
using UnityEngine;

public class SkillPointsUI : MonoBehaviour
{
    private TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        InvokeRepeating(nameof(Refresh), 0f, 0.1f);
    }

    void Refresh()
    {
        if (SkillTree.Instance != null)
            text.text = "Skill Points: " + SkillTree.Instance.availableSkillPoints;
    }
}