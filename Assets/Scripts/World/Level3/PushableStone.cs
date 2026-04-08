using UnityEngine;

public class PushableStone : MonoBehaviour
{
    public string stoneID;
    private Rigidbody rb;

    void Update()
    {
        Vector3 pos = transform.position;
        pos.y = startY;
        transform.position = pos;
    }
    
    private float startY;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation |
                         RigidbodyConstraints.FreezePositionY;
        startY = transform.position.y;
    }
    private void OnCollisionEnter(Collision col)
    {
        Debug.Log("Stone hit by: " + col.gameObject.name);
    }
}