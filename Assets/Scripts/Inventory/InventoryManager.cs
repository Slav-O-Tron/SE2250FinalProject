using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    private bool menuActivated;
    public ItemSlot[] itemSlot;

    [Header("Player Control")]
    public MonoBehaviour playerMovementScript;
    public MonoBehaviour playerLookScript;

    public bool MenuActivated
    {
        get { return menuActivated; }
    }

    void Update()
    {
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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (playerLookScript != null)
            playerLookScript.enabled = false;
    }

    void CloseInventory()
    {
        InventoryMenu.SetActive(false);
        menuActivated = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        if (playerLookScript != null)
            playerLookScript.enabled = true;
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