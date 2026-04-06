using UnityEngine;

/// <summary>
/// Abstract base for all world pickups (items, keys, coins, etc.).
/// Requires a Collider with "Is Trigger" checked on the GameObject.
/// </summary>
public abstract class Pickup : MonoBehaviour
{
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        OnPickedUp(other.transform.root.gameObject);
    }

    /// <summary>Called once when the player touches this pickup.</summary>
    protected abstract void OnPickedUp(GameObject player);
}
