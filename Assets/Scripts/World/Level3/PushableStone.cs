using UnityEngine;

public class PushableStone : MonoBehaviour
{
    public string stoneID;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation | 
                         RigidbodyConstraints.FreezePositionY;
    }
}