using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    public static SkillTree Instance;

    public int availableSkillPoints = 0;
    public List<string> unlockedSkills = new List<string>();

    private Player cachedPlayer;

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

    private Player GetPlayer()
    {
        cachedPlayer = null;
        Player[] allPlayers = FindObjectsByType<Player>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Debug.Log("Found " + allPlayers.Length + " Player instances:");
        foreach (Player p in allPlayers)
            Debug.Log(" - " + p.gameObject.name + " active: " + p.gameObject.activeInHierarchy + " moveSpeed: " + p.moveSpeed);
    
        foreach (Player p in allPlayers)
        {
            if (p.gameObject.activeInHierarchy)
            {
                cachedPlayer = p;
                return p;
            }
        }
        return null;
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

        Player p = player ?? GetPlayer();
        ApplySkillEffect(node, p);

        Debug.Log("Unlocked: " + node.skillName);
        RefreshAllNodes();

        return true;
    }

    private void ApplySkillEffect(SkillNode node, Player player)
    {
        
        Debug.Log("ApplySkillEffect called. Player: " + (player != null ? player.gameObject.name : "NULL"));
        if (player == null)
        {
            Debug.LogWarning("SkillTree: no player found");
            return;
        }
        Debug.Log("Before - attackDamage: " + player.attackDamage + " moveSpeed: " + player.moveSpeed);

        PlayerAbilities abilities = player.GetComponent<PlayerAbilities>();

        switch (node.skillID)
        {
            case "health_boost":
                player.AddMaxHealth(25);
                Debug.Log("Health boosted by 25");
                break;
            case "attack_boost":
                player.attackDamage += 5;
                Debug.Log("Attack damage: " + player.attackDamage);
                break;
            case "speed_boost":
                player.moveSpeed += 1f;
                player.sprintSpeed += 1.5f;
                Debug.Log("Speed boosted");
                break;
            case "double_jump":
                if (abilities != null) abilities.canDoubleJump = true;
                Debug.Log("Double jump unlocked");
                break;
            case "dash":
                if (abilities != null) abilities.canDash = true;
                Debug.Log("Dash unlocked");
                break;
            case "projectile_mastery":
                player.attackDamage += 3;
                Debug.Log("Projectile mastery: attack damage " + player.attackDamage);
                break;
            case "endure_hit":
                if (abilities != null)
                {
                    abilities.hasEndureHit = true;
                    abilities.endureHitAvailable = true;
                }
                Debug.Log("Endure Hit unlocked");
                break;
            case "damage_resistance":
                player.damageReduction += 0.1f;
                Debug.Log("Damage reduction: " + player.damageReduction);
                break;
            case "critical_strike":
                player.attackDamage += 8;
                Debug.Log("Critical strike: attack damage " + player.attackDamage);
                break;
        }
    }

    private void RefreshAllNodes()
    {
        foreach (SkillNodeUI nodeUI in FindObjectsByType<SkillNodeUI>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            nodeUI.Refresh();
    }
}