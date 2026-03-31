using UnityEngine;

/// <summary>
/// General-purpose world pickup for modular inventory/equipment.
/// 
/// Set in Inspector:
/// - itemData       -> the inventory item to add
/// - quantity       -> how many to add
/// - grantsDoorKey  -> optional temporary bool for old door logic
/// - coinValue      -> adds money directly
/// - xpValue        -> adds XP directly
/// </summary>
public class ItemPickup : Pickup
{
    [Header("Inventory Item")]
    [SerializeField] private ItemData itemData;
    [SerializeField] private int quantity = 1;

    [Header("Special Flags")]
    [SerializeField] private bool grantsDoorKey = false;

    [Header("Direct Rewards")]
    [SerializeField] private int coinValue = 0;
    [SerializeField] private int xpValue = 0;

    protected override void OnPickedUp(GameObject player)
    {
        // Add item to inventory
        if (itemData != null)
        {
            InventoryManager inventoryManager = GameObject.Find("InventoryCanvas")
                ?.GetComponent<InventoryManager>();

            if (inventoryManager != null)
                inventoryManager.AddItem(itemData, quantity);
            else
                Debug.LogWarning("ItemPickup: InventoryCanvas / InventoryManager not found.");
        }

        // Temporary support for old door-key logic
        if (grantsDoorKey)
        {
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory != null)
                inventory.hasDoorKey = true;
        }

        // Direct rewards
        Player playerEntity = player.GetComponent<Player>();

        if (coinValue > 0)
            playerEntity?.AddMoney(coinValue);

        if (xpValue > 0)
            playerEntity?.GainXP(xpValue); // change this if your XP method is elsewhere

        gameObject.SetActive(false);
    }
}