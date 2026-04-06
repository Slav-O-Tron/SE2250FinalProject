using UnityEngine;

public class ItemPickup : Pickup
{
    [Header("Inventory Item")]
    [SerializeField] private ItemData itemData;
    [SerializeField] private int quantity = 1;

    [Header("Direct Rewards")]
    [SerializeField] private int coinValue = 0;
    [SerializeField] private int xpValue = 0;

    protected override void OnPickedUp(GameObject player)
    {
        if (itemData != null)
        {
            InventoryManager inventoryManager = FindFirstObjectByType<InventoryManager>();

            if (inventoryManager != null)
                inventoryManager.AddItem(itemData, quantity);
            else
                Debug.LogWarning("ItemPickup: InventoryManager not found.");
        }

        Player playerEntity = player.GetComponent<Player>();

        if (coinValue > 0)
            playerEntity?.AddMoney(coinValue);

        if (xpValue > 0)
            playerEntity?.GainXP(xpValue);

        gameObject.SetActive(false);
    }
}