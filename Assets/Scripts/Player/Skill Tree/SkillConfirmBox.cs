using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillConfirmBox : MonoBehaviour
{
    public TextMeshProUGUI skillTitle;
    public TextMeshProUGUI skillDescription;
    public Button confirmButton;
    public TextMeshProUGUI confirmButtonText;

    private SkillNode pendingNode;
    private Player player;

    void Start()
    {
        player = FindFirstObjectByType<Player>();
        confirmButton.onClick.AddListener(OnConfirm);
        gameObject.SetActive(false);
    }

    public void Show(SkillNode node)
    {
        pendingNode = node;

        if (skillTitle != null)
            skillTitle.text = node.skillName;

        if (skillDescription != null)
            skillDescription.text = node.description;

        if (confirmButtonText != null)
            confirmButtonText.text = "Unlock — " + node.cost + " pt" + (node.cost > 1 ? "s" : "");

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        pendingNode = null;
        gameObject.SetActive(false);
    }

    private void OnConfirm()
    {
        if (pendingNode != null)
            pendingNode.Unlock(player);

        Hide();
    }
}