using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillData
{
    public string skillID;
    public string skillName;
    [TextArea] public string description;
    public int cost = 1;
    public List<string> prerequisiteSkillIDs = new List<string>();
}
