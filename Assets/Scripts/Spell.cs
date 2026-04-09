using UnityEngine;

public class Spell : MonoBehaviour
{
    public int damage = 20;

    private void OnCollisionEnter(Collision collision)
    {
        // 1. Check if the object hit has the "Player" tag
        if (collision.gameObject.CompareTag("Player"))
        {
            // Optional: Ignore the collision so the spell keeps flying
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            return; 
        }

        // 2. Only damage if it's an Entity and NOT the player
        if (collision.gameObject.TryGetComponent(out Entity entity))
        {
            entity.TakeDamage(damage);
        }

        // Destroy on impact with anything else (walls, enemies, etc.)
        Destroy(gameObject);
    }
}