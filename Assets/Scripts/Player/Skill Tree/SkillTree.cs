using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    public static SkillTree Instance;

    public int availableSkillPoints = 0;
    public List<string> unlockedSkills = new List<string>();

    private void Awake()
    {
        Debug.Log("SkillTree parent: " + (transform.parent == null ? "ROOT" : transform.parent.name));

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

    public bool CanUnlockSkill(SkillNode node)
    {
        if (node == null) return false;
        if (IsSkillUnlocked(node.skillID)) return false;
        if (availableSkillPoints < node.cost) return false;

        foreach (string prereq in node.prerequisiteSkillIDs)
        {
            if (!IsSkillUnlocked(prereq))
                return false;
        }

        return true;
    }

    public bool UnlockSkill(SkillNode node, Player player)
    {
        if (!CanUnlockSkill(node)) return false;

        availableSkillPoints -= node.cost;
        unlockedSkills.Add(node.skillID);

        ApplySkillEffect(node, player);

        Debug.Log("Unlocked: " + node.skillName);

        RefreshAllNodes();

        return true;
    }

    private void ApplySkillEffect(SkillNode node, Player player)
    {
        if (player == null) return;

        PlayerAbilities abilities = player.GetComponent<PlayerAbilities>();

        switch (node.skillID)
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
                if (abilities != null) abilities.canDoubleJump = true;
                break;
            case "dash":
                if (abilities != null) abilities.canDash = true;
                break;
            case "projectile_mastery":
                player.attackDamage += 3;
                break;
            case "crystal_resonance":
                if (abilities != null) abilities.hasCrystalResonance = true;
                break;
            case "damage_resistance":
                player.damageReduction += 0.1f;
                break;
            case "critical_strike":
                player.attackDamage += 8;
                break;
        }
    }

    private void RefreshAllNodes()
    {
        foreach (SkillNodeUI nodeUI in FindObjectsByType<SkillNodeUI>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            nodeUI.Refresh();
    }
}