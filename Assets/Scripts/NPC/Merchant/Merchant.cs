using UnityEngine;

/// <summary>
/// NPC Merchant class that inherits from Entity.
/// Handles buying and selling items to the player.
/// </summary>
public class Merchant : Entity
{
    [SerializeField] private string merchantName = "Merchant";
    [SerializeField] private int sellMarkup = 150; // Percentage: costs 150% of buy price
    
    /// <summary>Called when the merchant dies.</summary>
    protected override void OnDeath()
    {
        Debug.Log($"{merchantName} has been defeated!");
        // Add merchant-specific death behavior here (drop items, etc.)
        Destroy(gameObject);
    }

    /// <summary>Attempt to buy an item from the merchant.</summary>
    public bool TryBuyItem(int itemCost, out bool canAfford)
    {
        canAfford = money >= itemCost;
        if (canAfford)
        {
            money -= itemCost;
            return true;
        }
        return false;
    }

    /// <summary>Sell an item to the merchant.</summary>
    public void SellItem(int baseItemValue)
    {
        int sellPrice = (baseItemValue * sellMarkup) / 100;
        AddMoney(sellPrice);
        Debug.Log($"{merchantName} paid {sellPrice} coins for the item.");
    }
}
