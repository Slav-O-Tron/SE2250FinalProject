using UnityEngine;
public class PlayerCombat : MonoBehaviour
{
    public WeaponHitbox weaponHitbox; // Drag your Weapon object here in the Inspector
    public float attackDuration = 0.5f; // How long the hitbox stays "active"
    public float attackCooldown = 1f;
    private bool canAttack = true;
    public int attackDamage = 10; 
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            Attack();
        }
    }

    void Attack()
    {
        canAttack = false;
        Debug.Log("Attack Started");

        // 1. Turn the hitbox ON
        weaponHitbox.EnableHitbox();

        // 2. Schedule the hitbox to turn OFF
        Invoke(nameof(DisableHitbox), attackDuration);
        
        // 3. Schedule the next time you can click
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void DisableHitbox()
    {
        weaponHitbox.DisableHitbox();
    }

    void ResetAttack()
    {
        canAttack = true;
    }
    
    public int GetDamage()
    {
        return attackDamage;
    }
}