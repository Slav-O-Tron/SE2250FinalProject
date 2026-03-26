using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;

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
    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite,string itemDescription)
    {
        this.itemName = itemName;
        this.quantity = quantity;
        this.itemSprite = itemSprite;
        this.itemDescription = itemDescription;
        isFull = true;

        itemImage.sprite = this.itemSprite;
        itemImage.enabled = true;

        UpdateQuantityText();
    }

    public void AddQuantity(int amount)
    {
        quantity += amount;
        UpdateQuantityText();
    }

    private void UpdateQuantityText()
    {
        if (quantityText != null)
        {
            quantityText.text = quantity.ToString();
            quantityText.enabled = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }


    public void OnLeftClick()
        {
            inventoryManager.DeselectAllSlots();
            selectedShader.SetActive(true);
            thisItemSelected = true;
            ItemDescriptionNameText.text = itemName;
            ItemDescriptionText.text = itemDescription;
            ItemDescriptionImage.sprite = itemSprite;
        }
    
    public void OnRightClick(){


    }
    public void RemoveQuantity(int amount)
    {
        quantity -= amount;

        if (quantity <= 0)
        {
            ClearSlot();
        }
        else
        {
            UpdateQuantityText();
        }
    }

    public void ClearSlot()
    {
        itemName = "";
        quantity = 0;
        itemSprite = null;
        itemDescription = "";
        isFull = false;
        thisItemSelected = false;

        if (itemImage != null)
        {
            itemImage.sprite = null;
            itemImage.enabled = false;
        }

        if (quantityText != null)
        {
            quantityText.text = "";
            quantityText.enabled = false;
        }

        if (selectedShader != null)
            selectedShader.SetActive(false);

        if (ItemDescriptionNameText != null)
            ItemDescriptionNameText.text = "";

        if (ItemDescriptionText != null)
            ItemDescriptionText.text = "";

        if (ItemDescriptionImage != null)
            ItemDescriptionImage.sprite = null;
    }
}