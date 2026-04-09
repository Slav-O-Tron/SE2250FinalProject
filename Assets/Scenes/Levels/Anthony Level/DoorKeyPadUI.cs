using UnityEngine;

public class DoorKeypadUI : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject keypadUI;
    [SerializeField] private float showDistance = 3f;

    private bool isShowing = false;
    private KeyPad keypad;

    void Start()
    {
        if (keypadUI != null)
        {
            keypadUI.SetActive(false);
            keypad = keypadUI.GetComponentInChildren<KeyPad>(true);
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

            if (keypad != null)
                keypad.SetOpenState(true);
        }
        else if (!shouldShow && isShowing)
        {
            if (keypad != null)
            {
                keypad.SetOpenState(false);
                keypad.Clear();
            }

            keypadUI.SetActive(false);
            isShowing = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}