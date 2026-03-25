using UnityEngine;

/// <summary>
/// General-purpose world pickup. Replaces both Item.cs and DoorKeyPickup.cs.
///
/// Set fields in the Inspector per prefab:
///   - itemName / quantity / sprite / itemDescription  → shows up in inventory UI
///   - grantsDoorKey   → also sets PlayerInventory.hasDoorKey (for OpenDoor.cs)
///   - coinValue       → adds directly to player's Entity.money
///   - xpValue         → awards XP through Player.GainXP
///
/// Examples:
///   Door Key prefab   : grantsDoorKey = true, itemName = "Door Key"
///   Health Potion     : itemName = "Health Potion", quantity = 1
///   Coin pile         : coinValue = 5, leave itemName empty to skip inventory
/// </summary>
public class ItemPickup : Pickup
{
    [Header("Inventory Entry (leave Name empty to skip UI)")]
    [SerializeField] private string itemName;
    [SerializeField] private int quantity = 1;
    [SerializeField] private Sprite sprite;
    [SerializeField] private string itemDescription;

    [Header("Special Flags")]
    [SerializeField] private bool grantsDoorKey = false;

    [Header("Direct Rewards")]
    [SerializeField] private int coinValue = 0;
    [SerializeField] private int xpValue = 0;

    protected override void OnPickedUp(GameObject player)
    {
        // Inventory UI
        if (!string.IsNullOrEmpty(itemName))
        {
            InventoryManager inventoryManager = GameObject.Find("InventoryCanvas")
                ?.GetComponent<InventoryManager>();

            if (inventoryManager != null)
                inventoryManager.AddItem(itemName, quantity, sprite, itemDescription);
            else
                Debug.LogWarning("ItemPickup: InventoryCanvas not found in scene.");
        }

        // Door key flag
        if (grantsDoorKey)
        {
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory != null)
                inventory.hasDoorKey = true;
        }

        // Coins and XP via Player (which extends Entity)
        Player playerEntity = player.GetComponent<Player>();

        if (coinValue > 0)
            playerEntity?.AddMoney(coinValue);

        if (xpValue > 0)
            playerEntity?.GainXP(xpValue);

        gameObject.SetActive(false);
    }
}
