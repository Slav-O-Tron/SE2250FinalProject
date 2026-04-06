using UnityEngine;
 
[CreateAssetMenu(fileName = "NewProjectileItem", menuName = "Inventory/Projectile Item")]
public class ProjectileItem : ItemData
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 15f;
 
    public override void Use(Player player)
    {
        if (player == null || projectilePrefab == null) return;
 
        Transform firePoint = player.cameraTransform != null
            ? player.cameraTransform
            : player.transform;
 
        GameObject projectileObject = Object.Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );
 
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if (projectile != null)
            projectile.SetDirection(firePoint.forward, player.attackDamage);
    }
}