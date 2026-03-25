using UnityEngine;

public class IceShard : ProjectileItem
{
    private void Start()
    {
        itemName = "Ice Shard";
        itemDescription = "A sharpened shard of enchanted ice.";
    }

    public override void Use(Player player)
    {
        base.Use(player);
    }
}