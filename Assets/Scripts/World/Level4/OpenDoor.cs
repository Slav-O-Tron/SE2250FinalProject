using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform player;
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public float interactDistance = 3f;

    [Header("Key Settings")]
    public ItemData requiredKey;
    public bool consumeKey = false;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private InventoryManager inventoryManager;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
        inventoryManager = FindFirstObjectByType<InventoryManager>();
    }

    public void OpenFromKeypad()
    {
        Debug.Log("Door: OpenFromKeypad was called");
        isOpen = true;
    }

    void Update()
    {
        // Always handle door movement first
        if (isOpen)
            transform.rotation = Quaternion.Slerp(transform.rotation, openRotation, Time.deltaTime * openSpeed);
        else
            transform.rotation = Quaternion.Slerp(transform.rotation, closedRotation, Time.deltaTime * openSpeed);

        // Only handle manual key opening if player/inventory exist
        if (player == null || inventoryManager == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance && Input.GetKeyDown(KeyCode.E))
        {
            if (requiredKey == null)
            {
                Debug.LogWarning("Door has no required key assigned.");
                return;
            }

            if (inventoryManager.HasItem(requiredKey, 1))
            {
                if (consumeKey && !isOpen)
                {
                    inventoryManager.RemoveItem(requiredKey, 1);
                }

                isOpen = !isOpen;
                Debug.Log("Door toggled with key item.");
            }
            else
            {
                Debug.Log("You need the " + requiredKey.itemName + " to open this door.");
            }
        }
    }
}