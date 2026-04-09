using UnityEngine;

/// <summary>
/// Abstract base for all world pickups (items, keys, coins, etc.).
/// Requires a Collider with "Is Trigger" checked on the GameObject.
/// </summary>
public abstract class Pickup : MonoBehaviour
{
    private bool wasPickedUp;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (wasPickedUp)
        {
            return;
        }

        // Ignore triggers originating from within the same root hierarchy
        // (e.g. a player's equipped weapon triggering the pickup on their own body).
        if (other.transform.root == transform.root)
        {
            return;
        }

        Player player = ResolvePlayer(other);
        if (player == null)
        {
            return;
        }

        wasPickedUp = true;
        OnPickedUp(player.gameObject);
    }

    private static Player ResolvePlayer(Collider other)
    {
        if (other == null)
        {
            return null;
        }

        Player player = other.GetComponentInParent<Player>();
        if (player != null)
        {
            return player;
        }

        if (other.attachedRigidbody != null)
        {
            player = other.attachedRigidbody.GetComponentInParent<Player>();
            if (player != null)
            {
                return player;
            }
        }

        Transform root = other.transform.root;
        if (root != null)
        {
            player = root.GetComponent<Player>();
        }

        return player;
    }

    /// <summary>Called once when the player touches this pickup.</summary>
    protected abstract void OnPickedUp(GameObject player);
}
