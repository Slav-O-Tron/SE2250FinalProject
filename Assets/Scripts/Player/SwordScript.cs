using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    public Collider weaponCollider;
    private PlayerCombat combatOwner;
    private Player playerOwner;
    private bool ownerWarningLogged;

    private void Awake()
    {
        if (weaponCollider == null)
        {
            weaponCollider = GetComponent<Collider>();
        }

        if (weaponCollider == null)
        {
            Debug.LogWarning($"{name} is missing a weapon collider reference.", this);
        }

        RefreshOwnership();
        RefreshColliderState();
    }

    private void Start()
    {
        RefreshOwnership();
        RefreshColliderState();
        LogMissingOwnerWarning();
    }

    private void OnTransformParentChanged()
    {
        RefreshOwnership();
        RefreshColliderState();
    }

    public void InitializeOwner(Player owner, PlayerCombat combat)
    {
        playerOwner = owner;
        combatOwner = combat;
        ownerWarningLogged = false;
        RefreshColliderState();
    }

    private void RefreshOwnership()
    {
        if (combatOwner == null)
        {
            combatOwner = GetComponentInParent<PlayerCombat>();
        }

        if (playerOwner == null)
        {
            playerOwner = GetComponentInParent<Player>();
        }
    }

    private void RefreshColliderState()
    {
        if (weaponCollider == null)
        {
            return;
        }

        // Legacy Player scenes expect the sword hitbox to stay active,
        // while PlayerCombat scenes toggle it during attacks.
        weaponCollider.enabled = combatOwner == null && playerOwner != null;
    }

    private void LogMissingOwnerWarning()
    {
        if (!ownerWarningLogged && combatOwner == null && playerOwner == null)
        {
            ownerWarningLogged = true;
            Debug.LogWarning($"{name} WeaponHitbox has no player owner and will ignore collisions.", this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        RefreshOwnership();

        GameObject ownerObject = combatOwner != null ? combatOwner.gameObject : playerOwner != null ? playerOwner.gameObject : null;

        if (ownerObject == null)
        {
            LogMissingOwnerWarning();
            return;
        }

        Entity victim = other.GetComponentInParent<Entity>();

        if (victim != null && victim.gameObject != ownerObject && CanDamage(victim))
        {
            int damage = combatOwner != null ? combatOwner.GetDamage() : playerOwner.attackDamage;
            victim.TakeDamage(damage);
            Debug.Log($"Hit {other.name} for {damage} damage!");
        }
    }

    private static bool CanDamage(Entity victim)
    {
        return victim is not Player
            && victim is not Merchant
            && victim is not StoryNPC
            && victim is not ProtectedNPC;
    }

    public void EnableHitbox()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }
    }

    public void DisableHitbox()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }
}