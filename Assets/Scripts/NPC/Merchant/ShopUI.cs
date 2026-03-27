using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject itemButtonPrefab;
    [SerializeField] private Transform contentParent;

    private Player player;
    private InventoryManager inventoryManager;

    private void EnsureReferences()
    {
        if (player == null)
            player = FindFirstObjectByType<Player>();

        if (inventoryManager == null)
            inventoryManager = FindFirstObjectByType<InventoryManager>();
    }

    public void OpenShop(ShopItem[] items)
    {
        EnsureReferences();

        Debug.Log("OpenShop called");
        Debug.Log("Items length: " + (items == null ? 0 : items.Length));

        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (ShopItem shopItem in items)
        {
            if (shopItem == null)
            {
                Debug.Log("shopItem is null");
                continue;
            }

            if (shopItem.itemData == null)
            {
                Debug.Log("itemData is missing on ShopItem: " + shopItem.name);
                continue;
            }

            Debug.Log("Creating button for: " + shopItem.itemData.itemName);

            GameObject btn = Instantiate(itemButtonPrefab, contentParent);

            Transform imageTf = btn.transform.Find("ItemImage");
            Transform nameTf = btn.transform.Find("NameText");
            Transform priceTf = btn.transform.Find("PriceText");
            Transform buyTf = btn.transform.Find("BuyButton");

            if (imageTf != null)
                imageTf.GetComponent<Image>().sprite = shopItem.itemData.icon;

            if (nameTf != null)
                nameTf.GetComponent<TMP_Text>().text = shopItem.itemData.itemName;

            if (priceTf != null)
                priceTf.GetComponent<TMP_Text>().text = $"{shopItem.price} coins";

            ShopItem capturedItem = shopItem;

            if (buyTf != null)
            {
                Button buyButton = buyTf.GetComponent<Button>();
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(() => TryBuy(capturedItem));
            }
        }

        gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseShop()
    {
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void TryBuy(ShopItem shopItem)
    {
        if (player == null || inventoryManager == null || shopItem == null || shopItem.itemData == null)
            return;

        if (player.SpendMoney(shopItem.price))
        {
            inventoryManager.AddItem(shopItem.itemData, shopItem.quantity);
            Debug.Log($"Bought {shopItem.itemData.itemName} for {shopItem.price} coins.");
        }
        else
        {
            Debug.Log("Not enough coins.");
        }
    }
}