using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// Attach to your ShopPanel UI GameObject.
/// Scene setup:
///   Canvas
///   └── ShopPanel (attach ShopUI here)
///       ├── ItemButtonPrefab slots (assign prefab below)
///       ├── ContentParent  (the layout group that holds item buttons)
///       └── CloseButton
public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject itemButtonPrefab;   // Prefab with Image + NameText + PriceText + BuyButton
    [SerializeField] private Transform contentParent;       // Layout group inside ShopPanel

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

        // Clear old buttons
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (ShopItem item in items)
        {
            GameObject btn = Instantiate(itemButtonPrefab, contentParent);

            btn.transform.Find("ItemImage").GetComponent<Image>().sprite = item.itemSprite;
            btn.transform.Find("NameText").GetComponent<TMP_Text>().text = item.itemName;
            btn.transform.Find("PriceText").GetComponent<TMP_Text>().text = $"{item.price} coins";

            ShopItem captured = item;
            btn.transform.Find("BuyButton").GetComponent<Button>().onClick.AddListener(() => TryBuy(captured));
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

    private void TryBuy(ShopItem item)
    {
        if (player == null || inventoryManager == null) return;

        if (player.SpendMoney(item.price))
        {
            inventoryManager.AddItem(item.itemName, item.quantity, item.itemSprite, item.itemDescription);
            Debug.Log($"Bought {item.itemName} for {item.price} coins.");
        }
        else
        {
            Debug.Log("Not enough coins.");
        }
    }
}
