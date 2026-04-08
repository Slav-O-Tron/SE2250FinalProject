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
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (ShopItem shopItem in items)
        {
            if (shopItem == null) continue;

            GameObject btn = Instantiate(itemButtonPrefab, contentParent);
            Transform imageTf = btn.transform.Find("ItemImage");
            Transform nameTf = btn.transform.Find("NameText");
            Transform priceTf = btn.transform.Find("PriceText");
            Transform buyTf = btn.transform.Find("BuyButton");

            if (shopItem.isSkillPoints)
            {
                if (imageTf != null)
                    imageTf.GetComponent<Image>().sprite = null;
                if (nameTf != null)
                    nameTf.GetComponent<TMP_Text>().text = $"+{shopItem.skillPointAmount} Skill Point(s)";
                if (priceTf != null)
                    priceTf.GetComponent<TMP_Text>().text = $"{shopItem.price} coins";
            }
            else
            {
                if (shopItem.itemData == null) continue;
                if (imageTf != null)
                    imageTf.GetComponent<Image>().sprite = shopItem.itemData.icon;
                if (nameTf != null)
                    nameTf.GetComponent<TMP_Text>().text = shopItem.itemData.itemName;
                if (priceTf != null)
                    priceTf.GetComponent<TMP_Text>().text = $"{shopItem.price} coins";
            }

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
        if (player == null || shopItem == null) return;

        if (shopItem.isSkillPoints)
        {
            if (player.SpendMoney(shopItem.price))
            {
                SkillTree.Instance?.AddSkillPoints(shopItem.skillPointAmount);
                Debug.Log($"Bought {shopItem.skillPointAmount} skill point(s) for {shopItem.price} coins.");
            }
            else
                Debug.Log("Not enough coins.");
            return;
        }

        if (inventoryManager == null || shopItem.itemData == null) return;

        if (player.SpendMoney(shopItem.price))
        {
            inventoryManager.AddItem(shopItem.itemData, shopItem.quantity);
            Debug.Log($"Bought {shopItem.itemData.itemName} for {shopItem.price} coins.");
        }
        else
            Debug.Log("Not enough coins.");
    }
}