using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform player;
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public float interactDistance = 3f;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private PlayerInventory inventory;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));

        if (player != null)
        {
            inventory = player.GetComponent<PlayerInventory>();
        }
    }

    void Update()
    {
        if (player == null) return;
        if (inventory == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance && Input.GetKeyDown(KeyCode.E))
        {
            if (inventory.hasDoorKey)
            {
                isOpen = !isOpen;
            }
            else
            {
                Debug.Log("You need a key to open this door.");
            }
        }

        if (isOpen)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, openRotation, Time.deltaTime * openSpeed);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, closedRotation, Time.deltaTime * openSpeed);
        }
    }
}