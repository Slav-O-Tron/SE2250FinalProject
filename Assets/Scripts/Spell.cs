using UnityEngine;

public class Spell : MonoBehaviour
{
    public int damage = 20;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Entity entity))
        {
            entity.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
