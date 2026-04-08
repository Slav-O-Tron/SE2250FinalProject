using UnityEngine;

public class ChronoShardCarrier : MonoBehaviour
{
    private ItemData itemData;
    private int quantity = 1;
    private bool dropped;

    public void Configure(ItemData item, int amount = 1)
    {
        itemData = item;
        quantity = Mathf.Max(1, amount);
    }

    public void Drop()
    {
        if (dropped || itemData == null)
            return;

        dropped = true;

        GameObject pickupObject = new GameObject(string.IsNullOrEmpty(itemData.itemName) ? "Chrono Shard Pickup" : itemData.itemName + " Pickup");
        pickupObject.transform.position = transform.position + Vector3.up * 0.5f;

        SphereCollider trigger = pickupObject.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 0.75f;

        ItemPickup pickup = pickupObject.AddComponent<ItemPickup>();
        pickup.Initialize(itemData, quantity);

        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visual.name = "Visual";
        visual.transform.SetParent(pickupObject.transform, false);
        visual.transform.localScale = Vector3.one * 0.35f;

        Collider visualCollider = visual.GetComponent<Collider>();
        if (visualCollider != null)
            Object.Destroy(visualCollider);

        Renderer visualRenderer = visual.GetComponent<Renderer>();
        if (visualRenderer != null)
            visualRenderer.material.color = new Color(0.3f, 0.9f, 1f, 1f);
    }
}