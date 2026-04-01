using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    public static SkillTree Instance;

    public int availableSkillPoints = 0;
    public List<string> unlockedSkills = new List<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddSkillPoints(int amount)
    {
        availableSkillPoints += amount;
        Debug.Log("Skill points: " + availableSkillPoints);
    }

    public bool IsSkillUnlocked(string skillID)
    {
        return unlockedSkills.Contains(skillID);
    }

    public bool CanUnlockSkill(SkillData skill)
    {
        if (skill == null) return false;
        if (IsSkillUnlocked(skill.skillID)) return false;
        if (availableSkillPoints < skill.cost) return false;

        foreach (string prereq in skill.prerequisiteSkillIDs)
        {
            if (!IsSkillUnlocked(prereq))
                return false;
        }

        return true;
    }

    public bool UnlockSkill(SkillData skill, Player player)
    {
        if (!CanUnlockSkill(skill)) return false;

        availableSkillPoints -= skill.cost;
        unlockedSkills.Add(skill.skillID);

        ApplySkillEffect(skill, player);

        Debug.Log("Unlocked: " + skill.skillName);

        RefreshAllNodes();

        return true;
    }

    private void ApplySkillEffect(SkillData skill, Player player)
    {
        if (player == null) return;

        PlayerAbilities abilities = player.GetComponent<PlayerAbilities>();

        switch (skill.skillID)
        {
            case "health_boost":
                player.AddMaxHealth(25);
                break;

            case "attack_boost":
                player.attackDamage += 5;
                break;

            case "speed_boost":
                player.moveSpeed += 1f;
                player.sprintSpeed += 1.5f;
                break;

            case "double_jump":
                if (abilities != null)
                    abilities.canDoubleJump = true;
                break;

            case "dash":
                if (abilities != null)
                    abilities.canDash = true;
                break;

            case "projectile_mastery":
                player.attackDamage += 3;
                break;

            case "crystal_resonance":
                if (abilities != null)
                    abilities.hasCrystalResonance = true;
                break;

            case "damage_resistance":
                player.damageReduction += 0.1f;
                break;
        }
    }

    private void RefreshAllNodes()
    {
        foreach (SkillNodeUI node in FindObjectsByType<SkillNodeUI>(FindObjectsSortMode.None))
            node.Refresh();
    }
}