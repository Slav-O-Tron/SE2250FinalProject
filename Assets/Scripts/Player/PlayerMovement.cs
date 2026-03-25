using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float baseSpeed = 5f;

    public float _mouseSensitivity = 2f;

    private Vector3 moveInput;

    private float rotationY;

    private float defaultScale;

    void Start()
    {
        defaultScale = transform.localScale.x;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveInput = new Vector3(horizontal, 0f, vertical).normalized;

        //Scale speed proportionally to the character's current size.
        float scaleMultiplier = transform.localScale.x / defaultScale;
        float speed = baseSpeed * scaleMultiplier;

        transform.Translate(moveInput * speed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");

        rotationY += mouseX * _mouseSensitivity;

        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }
}