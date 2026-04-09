
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float distance = 5f;
    public float height = 2f;
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (player == null) return;

        // Position camera behind and above the player
        Vector3 targetPos = player.position + new Vector3(0, height, -distance);

        // Smoothly move camera to target
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);

        // Look at the player
        transform.LookAt(player.position + Vector3.up * (height * 0.5f));
    }
}
