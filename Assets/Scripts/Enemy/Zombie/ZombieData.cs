using UnityEngine;

/// ScriptableObject config for Zombie stats.
/// Create one asset per level to scale difficulty without touching code.

public class ZombieData : ScriptableObject
{
    [Header("Health")]
    public int maxHealth = 50;

    [Header("Combat")]
    public int attackDamage = 10;
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float detectRange = 6f;

    [Header("Rewards on Death")]
    [Tooltip("Coins awarded to player. Picked randomly between min and max.")]
    public int coinDropMin = 1;
    public int coinDropMax = 10;
    public int xpReward = 20;
}