using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    public Collider weaponCollider;
    private PlayerCombat owner;

    void Start()
    {
        owner = GetComponentInParent<PlayerCombat>();
        weaponCollider.enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        // 1. Check if physics is working at all
        Debug.Log("Physics hit detected with: " + other.name);
        // Try to find ANY entity on the object we hit
        
        Entity victim = other.GetComponent<Entity>();

        // Make sure we aren't hitting ourselves and the target exists
        if (victim != null && victim.gameObject != owner.gameObject)
        {
            victim.TakeDamage(owner.GetDamage());
            Debug.Log($"Hit {other.name} for {owner.GetDamage()} damage!");
        }
        
    }

    public void EnableHitbox()
    {
        weaponCollider.enabled = true;
    }

    public void DisableHitbox()
    {
        weaponCollider.enabled = false;
    }
}