using System;
using UnityEngine;

public class Item : MonoBehaviour
{

    [SerializeField] private string itemName;

    [SerializeField] private int quantity;
    
    [SerializeField] private Sprite sprite;
    
    [SerializeField] private string itemDescription;

    [TextArea] InventoryManager inventoryManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            inventoryManager.AddItem(itemName, quantity, sprite,itemDescription);
            Destroy(gameObject);
            
        }
    }
}

