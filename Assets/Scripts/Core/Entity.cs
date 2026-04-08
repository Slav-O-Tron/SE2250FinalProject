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

    /// Fired whenever health changes: (currentHealth, maxHealth)
    public event System.Action<int, int> OnHealthChanged;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    /// Apply damage. Override to add armor, resistances, etc.
    public virtual void TakeDamage(int amount)
    {
        if (!IsAlive) return;
        //for Endure hit in SkillTree
        PlayerAbilities abilities = GetComponent<PlayerAbilities>();
        int newHealth = currentHealth - amount;

        if (abilities != null && abilities.hasEndureHit && abilities.endureHitAvailable && newHealth <= 0)
        {
            currentHealth = 1;
            abilities.endureHitAvailable = false;
            Debug.Log("Endure Hit triggered - survived with 1 HP!");
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnDamageTaken(amount);
            return;
        }
        
        
        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnDamageTaken(amount);
        if (currentHealth <= 0)
            OnDeath();
    }

    /// estore health, capped at maxHealth.
    public virtual void Heal(int amount)
    {
        if (!IsAlive) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
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
    
    //adding max health so that health potions can function
    public void AddMaxHealth(int amount)
    {
        maxHealth     += amount;
        currentHealth += amount;
    }

    protected virtual void OnDamageTaken(int amount) { }
    protected abstract void OnDeath();
}