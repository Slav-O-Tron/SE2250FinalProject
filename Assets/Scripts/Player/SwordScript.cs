using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    public Collider weaponCollider;

    private PlayerCombat playerCombat;

    void Start()
    {
        playerCombat = GetComponentInParent<PlayerCombat>();
        weaponCollider.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        // 1. Check if physics is working at all
        Debug.Log("Physics hit detected with: " + other.name);

        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        
            // 2. Check if we found the script
            if (enemy == null) {
                Debug.LogError("Found Enemy tag, but EnemyHealth script is missing on " + other.name);
                return;
            }

            // 3. Check if we have the player reference
            if (playerCombat == null) {
                Debug.LogError("WeaponHitbox cannot find PlayerCombat in parent objects!");
                return;
            }

            enemy.TakeDamage(playerCombat.GetDamage());
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