using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [TextArea] public string description;

    public bool isStackable = false;
    public bool isEquipable = false;
    public EquipmentSlot equipmentSlot = EquipmentSlot.None;

    [Header("Armor prefab to spawn on player")]
    public GameObject equipmentPrefab;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            inventoryManager.AddItem(itemName, quantity, sprite,itemDescription);
            Destroy(gameObject);
            
        }
    }
    
    public virtual void Use(Player player){}
    public virtual void OnPickup(Player player){}
}


