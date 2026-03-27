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
}