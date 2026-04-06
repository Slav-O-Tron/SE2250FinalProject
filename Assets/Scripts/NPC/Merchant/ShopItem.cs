using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Shop Item")]
public class ShopItem : ScriptableObject
{
    public ItemData itemData;
    public int price = 10;
    public int quantity = 1;
}