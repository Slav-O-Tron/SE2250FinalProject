using System.Collections.Generic;
using UnityEngine;

public class SkillNode : MonoBehaviour
{
    [Header("Skill Info")]
    public string skillID;
    public string skillName;
    [TextArea] public string description;
    public int cost = 1;

    [Header("Prerequisites")]
    public List<string> prerequisiteSkillIDs = new List<string>();

    public SkillData GetSkillData()
    {
        SkillData data = new SkillData();
        data.skillID              = skillID;
        data.skillName            = skillName;
        data.description          = description;
        data.cost                 = cost;
        data.prerequisiteSkillIDs = prerequisiteSkillIDs;
        return data;
    }

    public bool IsUnlocked()
    {
        return SkillTree.Instance.IsSkillUnlocked(skillID);
    }

    public bool CanUnlock()
    {
        return SkillTree.Instance.CanUnlockSkill(GetSkillData());
    }

    public void Unlock(Player player)
    {
        SkillTree.Instance.UnlockSkill(GetSkillData(), player);
    }
}