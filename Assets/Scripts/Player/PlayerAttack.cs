using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public WeaponHitbox weaponHitbox;
    public float attackDuration = 0.5f;
    public float attackCooldown = 1f;
    public int attackDamage = 10;

    private bool canAttack = true;
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // right CLICK
        if (Input.GetMouseButtonDown(1) && canAttack)
        {
            Attack();
        }
    }

    void Attack()
    {
        canAttack = false;

        Debug.Log("Attack Started");

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        weaponHitbox.EnableHitbox();

        Invoke(nameof(DisableHitbox), attackDuration);
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