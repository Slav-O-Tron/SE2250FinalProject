using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public ItemData itemData;
    public string itemName;
    public int quantity;
    public bool isFull;

    public GameObject selectedShader;
    public bool thisItemSelected;

    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;

    private InventoryManager inventoryManager;

    public Image ItemDescriptionImage;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;

    private void Start()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();

        if (selectedShader != null)
            selectedShader.SetActive(false);

        RefreshUI();
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        bool hasItem = isFull && itemData != null;

        if (itemImage != null)
        {
            itemImage.enabled = true;

            if (hasItem)
            {
                itemImage.sprite = itemData.icon;
                itemImage.color = Color.white;
                itemImage.preserveAspect = true;
                itemImage.type = Image.Type.Simple;
            }
            else
            {
                itemImage.sprite = null;
            }
        }

        if (quantityText != null)
        {
            quantityText.text = hasItem && quantity > 1 ? quantity.ToString() : "";
        }
    }

    public void AddItem(ItemData newItem, int amount)
    {
        Debug.Log("Adding icon to slot: " + newItem.itemName + " | itemImage = " + itemImage);

        itemData = newItem;
        itemName = newItem.itemName;
        quantity = amount;
        isFull = true;

        RefreshUI();
    }

    public void AddQuantity(int amount)
    {
        quantity += amount;
        RefreshUI();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            OnLeftClick();

        if (eventData.button == PointerEventData.InputButton.Right)
            OnRightClick();
    }

    public void OnLeftClick()
    {
        if (!isFull || itemData == null) return;

        inventoryManager.DeselectAllSlots();

        if (selectedShader != null)
            selectedShader.SetActive(true);

        thisItemSelected = true;

        if (ItemDescriptionNameText != null)
            ItemDescriptionNameText.text = itemData.itemName;

        if (ItemDescriptionText != null)
            ItemDescriptionText.text = itemData.description;

        if (ItemDescriptionImage != null)
            ItemDescriptionImage.sprite = itemData.icon;
    }

    public void OnRightClick()
    {
        if (!isFull || itemData == null) return;
        if (!itemData.isEquipable) return;

        EquipmentManager equipmentManager = FindFirstObjectByType<EquipmentManager>();
        if (equipmentManager != null)
            equipmentManager.ToggleEquip(itemData);
    }

    public void RemoveQuantity(int amount)
    {
        quantity -= amount;

        if (quantity <= 0)
            ClearSlot();
        else
            RefreshUI();
    }

    public void ClearSlot()
    {
        itemData = null;
        itemName = "";
        quantity = 0;
        isFull = false;
        thisItemSelected = false;

        if (selectedShader != null)
            selectedShader.SetActive(false);

        if (ItemDescriptionNameText != null)
            ItemDescriptionNameText.text = "";

        if (ItemDescriptionText != null)
            ItemDescriptionText.text = "";

        if (ItemDescriptionImage != null)
            ItemDescriptionImage.sprite = null;

        RefreshUI();
    }
}