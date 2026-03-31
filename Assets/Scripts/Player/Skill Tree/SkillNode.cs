using UnityEngine;

public class SkillNode : MonoBehaviour
{
    public SkillData skillData;

    public bool IsUnlocked()
    {
        return SkillTree.Instance.IsSkillUnlocked(skillData.skillID);
    }

    public bool CanUnlock()
    {
        return SkillTree.Instance.CanUnlockSkill(skillData);
    }

    public void Unlock(Player player)
    {
        SkillTree.Instance.UnlockSkill(skillData, player);
    }
}
