using UnityEngine;

/// Base class for all entities in the game world (Player, Enemy, NPC).
/// Owns: Health, Money, Position.

public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected int money = 0;

    public int MaxHealth     => maxHealth;
    public int CurrentHealth => currentHealth;
    public int Money         => money;
    public bool IsAlive      => currentHealth > 0;
    public Vector3 Position  => transform.position;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    /// <summary>Apply damage. Override to add armor, resistances, etc.</summary>
    public virtual void TakeDamage(int amount)
    {
        if (!IsAlive) return;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnDamageTaken(amount);
        if (currentHealth <= 0)
            OnDeath();
    }

    /// <summary>Restore health, capped at maxHealth.</summary>
    public virtual void Heal(int amount)
    {
        if (!IsAlive) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }

    public virtual void AddMoney(int amount)
    {
        money += amount;
        Debug.Log($"{name} gained {amount} coins. Total = {money}");
    }

    /// <summary>Returns false if insufficient funds.</summary>
    public virtual bool SpendMoney(int amount)
    {
        if (money < amount) return false;
        money -= amount;
        return true;
    }

    protected virtual void OnDamageTaken(int amount) { }
    protected abstract void OnDeath();
}