using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    public static SkillTree Instance;

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
        if (cachedPlayer != null && cachedPlayer.gameObject.activeInHierarchy)
            return cachedPlayer;

        Player[] allPlayers = FindObjectsByType<Player>(FindObjectsInactive.Include, FindObjectsSortMode.None);
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

    private PlayerInventory GetPlayerInventory()
    {
        Player p = GetPlayer();
        return p != null ? p.GetComponent<PlayerInventory>() : null;
    }

    /// Returns the player's current level — this is the currency for buying skills.
    public int GetAvailableLevels()
    {
        PlayerInventory inv = GetPlayerInventory();
        return inv != null ? inv.level : 0;
    }

    private void SpendLevels(int amount)
    {
        PlayerInventory inv = GetPlayerInventory();
        if (inv == null) return;
        inv.level = Mathf.Max(0, inv.level - amount);
        inv.SaveToPlayerData();
    }

    public bool IsSkillUnlocked(string skillID)
    {
        return unlockedSkills.Contains(skillID);
    }

    public bool CanUnlockSkill(SkillNode node)
    {
        if (node == null) return false;
        if (IsSkillUnlocked(node.skillID)) return false;
        if (GetAvailableLevels() < node.cost) return false;

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

        SpendLevels(node.cost);
        unlockedSkills.Add(node.skillID);

        Player p = player ?? GetPlayer();
        ApplySkillEffect(node, p);

        Debug.Log("Unlocked: " + node.skillName);
        RefreshAllNodes();

        return true;
    }

    private void ApplySkillEffect(SkillNode node, Player player)
    {
        if (player == null)
        {
            Debug.LogWarning("SkillTree: no player found to apply effect to");
            return;
        }

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
            case "endure_hit":
                if (abilities != null)
                {
                    abilities.hasEndureHit = true;
                    abilities.endureHitAvailable = true;
                }
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
