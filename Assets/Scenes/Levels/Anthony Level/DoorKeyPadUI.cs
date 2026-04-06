using UnityEngine;

public class DoorKeypadUI : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject keypadUI;
    [SerializeField] private float showDistance = 3f;

    private bool isShowing = false;

    void Start()
    {
        if (keypadUI != null)
        {
            keypadUI.SetActive(false); // start hidden
        }
    }

    void Update()
    {
        if (player == null || keypadUI == null) return;

        float distance = Vector3.Distance(player.position, transform.position);
        bool shouldShow = distance <= showDistance;

        if (shouldShow && !isShowing)
        {
            keypadUI.SetActive(true);
            isShowing = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (!shouldShow && isShowing)
        {
            keypadUI.SetActive(false);
            isShowing = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            KeyPad keypad = keypadUI.GetComponentInChildren<KeyPad>(true);
            if (keypad != null)
            {
                keypad.Clear();
            }
        }
    }
}