using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public float speed = 5f;
    
    public float _mouseSensitivity = 2f; 
    private Vector3 moveInput;
    private float rotationY;
    
    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        moveInput = new Vector3(horizontal, 0f, vertical).normalized;
        
        transform.Translate(moveInput * speed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        rotationY +=  mouseX * _mouseSensitivity;
        
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }
    
}
