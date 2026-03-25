using UnityEngine;

public class FireFlask : ProjectileItem
{
    private void Start()
    {
        itemName = "Fire Flask";
        itemDescription = "A volatile flask that bursts into magical flame.";
    }

    public override void Use(Player player)
    {
        base.Use(player);
    }
}