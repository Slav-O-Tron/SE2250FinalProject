using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "SkillTree/Skill")]
public class SkillData : ScriptableObject
{
    public string skillID;
    public string skillName;
    [TextArea] public string description;
    public int cost = 1;
    public List<string> prerequisiteSkillIDs = new List<string>();
}