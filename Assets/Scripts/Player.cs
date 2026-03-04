using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 4f;
    CharacterController controller;
    private Vector3 moveInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        moveInput.x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        moveInput.z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
             
        controller.Move(moveInput);
     
    }
}
