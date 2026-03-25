using UnityEngine;

public class ProjectileItem : Item
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 15f;
    public Transform firePoint;

    public override void Use(Player player)
    {
        if (player == null || projectilePrefab == null || firePoint == null)
            return;

        GameObject projectileObject = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.speed = projectileSpeed;
            projectile.SetDirection(firePoint.forward, player.attackDamage);
        }

        quantity--;
        if (quantity <= 0)
        {
            Destroy(gameObject);
        }
    }
}