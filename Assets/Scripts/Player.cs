using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float sprintSpeed = 8f;
    public float gravity = 9.81f;
    public float jumpHeight = 10; 
    
    
    CharacterController controller;
    private Vector3 moveInput;
    private float verticalVelocity;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        InputManagement();
    }

    private void InputManagement()
    {
        moveInput.x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        moveInput.z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        moveInput.y = VerticalForceCalculation();
        controller.Move(moveInput);
    }
    
    private float VerticalForceCalculation(){
    if(controller.isGrounded)
    {
        verticalVelocity = -1f;
        if(Input.GetButtonDown("Jump"))
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * gravity * 2);
        }
    }
    else
    {
        verticalVelocity -= gravity * Time.deltaTime;
    }

    return verticalVelocity;
    }
    
    
}
