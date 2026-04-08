using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    private bool menuActivated;
    private bool externalMenuActive;
    public ItemSlot[] itemSlot;

    [Header("Player Control")]
    public MonoBehaviour playerMovementScript;
    public MonoBehaviour playerLookScript;

    public bool MenuActivated
    {
        get { return menuActivated || externalMenuActive; }
    }

    private void Start()
    {
        RestoreFromPlayerData();
    }

    private void OnDestroy()
    {
        SaveToPlayerData();
    }

    private void SaveToPlayerData()
    {
        PlayerData pd = PlayerData.GetOrCreate();
        pd.savedInventory.Clear();
        foreach (ItemSlot slot in itemSlot)
        {
            if (slot.isFull && slot.itemData != null)
            {
                pd.savedInventory.Add(new PlayerData.InventorySaveEntry
                {
                    item = slot.itemData,
                    quantity = slot.quantity
                });
            }
        }
    }

    private void RestoreFromPlayerData()
    {
        PlayerData pd = PlayerData.GetOrCreate();
        if (pd.savedInventory == null || pd.savedInventory.Count == 0) return;

        foreach (PlayerData.InventorySaveEntry entry in pd.savedInventory)
        {
            if (entry.item != null)
                AddItem(entry.item, entry.quantity);
        }
    }

    void Update()
    {
        if (externalMenuActive)
            return;

        if (Input.GetButtonDown("Inventory") && menuActivated)
        {
            CloseInventory();
        }
        else if (Input.GetButtonDown("Inventory") && !menuActivated)
        {
            OpenInventory();
        }
    }

    void OpenInventory()
    {
        InventoryMenu.SetActive(true);
        menuActivated = true;
        ApplyMenuState();
    }

    void CloseInventory()
    {
        InventoryMenu.SetActive(false);
        menuActivated = false;

        ApplyMenuState();
    }

    public void SetExternalMenuActive(bool isActive)
    {
        externalMenuActive = isActive;
        ApplyMenuState();
    }

    private void ApplyMenuState()
    {
        bool anyMenuOpen = menuActivated || externalMenuActive;

        Cursor.lockState = anyMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = anyMenuOpen;

        if (playerMovementScript != null)
            playerMovementScript.enabled = !anyMenuOpen;

        if (playerLookScript != null)
            playerLookScript.enabled = !anyMenuOpen;
    }

    public void AddItem(ItemData item, int quantity)
    {
        if (item == null) return;

        if (item.isStackable)
        {
            for (int i = 0; i < itemSlot.Length; i++)
            {
                if (itemSlot[i].isFull && itemSlot[i].itemData == item)
                {
                    itemSlot[i].AddQuantity(quantity);
                    return;
                }
            }
        }

        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (!itemSlot[i].isFull)
            {
                itemSlot[i].AddItem(item, quantity);
                return;
            }
        }
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i] != null)
            {
                itemSlot[i].thisItemSelected = false;

                if (itemSlot[i].selectedShader != null)
                    itemSlot[i].selectedShader.SetActive(false);
            }
        }
    }

    public bool RemoveItem(ItemData item, int quantity)
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].isFull && itemSlot[i].itemData == item)
            {
                if (itemSlot[i].quantity >= quantity)
                {
                    itemSlot[i].RemoveQuantity(quantity);
                    return true;
                }

                return false;
            }
        }

        return false;
    }

    public bool HasItem(ItemData item, int quantity = 1)
    {
        if (item == null) return false;

        int total = 0;

        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].isFull && itemSlot[i].itemData == item)
            {
                total += itemSlot[i].quantity;
                if (total >= quantity)
                    return true;
            }
        }

        return false;
    }
}