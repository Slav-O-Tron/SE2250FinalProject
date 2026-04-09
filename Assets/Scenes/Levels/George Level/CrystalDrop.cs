using UnityEngine;

public class CrystalDrop : MonoBehaviour
{
    [SerializeField] private ItemData chronoCrystalItemData;
    [SerializeField] private int dropAmount = 1;

    private Entity entity;
    private bool dropped = false;

    void Start()
    {
        entity = GetComponent<Entity>();
    }

    void Update()
    {
        if (!dropped && entity != null && !entity.IsAlive)
        {
            dropped = true;
            DropCrystal();
        }
    }

    private void DropCrystal()
    {
        InventoryManager inventoryManager = FindFirstObjectByType<InventoryManager>();
        if (inventoryManager != null && chronoCrystalItemData != null)
        {
            inventoryManager.AddItem(chronoCrystalItemData, dropAmount);
            Debug.Log($"Dropped {dropAmount} Chrono Crystal(s)");
        }
        else
            Debug.LogWarning("CrystalDrop: InventoryManager or ItemData not found");
    }
}