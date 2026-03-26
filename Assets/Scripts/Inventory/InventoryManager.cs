using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    private bool menuActivated;
    public ItemSlot[] itemSlot;

    public bool MenuActivated
    {
        get { return menuActivated; }
    }

    void Update()
    {
        if (Input.GetButtonDown("Inventory") && menuActivated)
        {
            InventoryMenu.SetActive(false);
            menuActivated = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (Input.GetButtonDown("Inventory") && !menuActivated)
        {
            InventoryMenu.SetActive(true);
            menuActivated = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].isFull && itemSlot[i].itemName == itemName)
            {
                itemSlot[i].AddQuantity(quantity);
                return;
            }
        }

        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (!itemSlot[i].isFull)
            {
                itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription);
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
                {
                    itemSlot[i].selectedShader.SetActive(false);
                }
            }
        }
    }
    public bool RemoveItem(string itemName, int quantity)
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].isFull && itemSlot[i].itemName == itemName)
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
}