using UnityEngine;

public class DoorKeyPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();

        if (inventory != null)
        {
            inventory.hasDoorKey = true;
            Debug.Log("Picked up the key!");
            gameObject.SetActive(false);
        }
    }
}