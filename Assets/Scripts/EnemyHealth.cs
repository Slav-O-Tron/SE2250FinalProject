using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 30;

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log(gameObject.name + " took " + amount + " damage.");

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}