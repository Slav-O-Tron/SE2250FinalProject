using UnityEngine;

public class Wand : MonoBehaviour
{
    public int damage = 20;
    public float fireRate = 0.5f;
    public float spread = 0.05f;

    public Transform attackPoint;
    public GameObject spellPrefab;
    public float spellSpeed = 20f;

    public ParticleSystem castEffect;
    public AudioSource audioSource;
    public AudioClip castSound;
    private float nextFireTime;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    // Call this from your Update() method when the fire button is pressed
    public void CastSpell()
    {
        // 1. Enforce the fire rate
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireRate;

        // 2. Get the EXACT center of the screen, ignoring the mouse cursor
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = cam.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            targetPoint = hit.point;
        }
        else
        {
            // If the ray doesn't hit anything, pick a point far in the distance
            targetPoint = ray.GetPoint(100f);
        }

        // 3. Calculate the direction from the wand tip to the camera's target
        Vector3 direction = (targetPoint - attackPoint.position).normalized;

        // 4. Wall-hugging fix: Prevent shooting backward if the target is behind the wand tip
        if (Vector3.Dot(direction, cam.transform.forward) < 0)
        {
            direction = cam.transform.forward;
        }

        // 5. Add spread
        direction += cam.transform.right * Random.Range(-spread, spread);
        direction += cam.transform.up * Random.Range(-spread, spread);
        direction = direction.normalized;

        // Play effects
        if (castEffect != null) castEffect.Play();
        if (audioSource != null && castSound != null) audioSource.PlayOneShot(castSound);

        // 6. Spawn the spell AND rotate it to face its travel direction
        GameObject spell = Instantiate(spellPrefab, attackPoint.position, Quaternion.LookRotation(direction));

        Rigidbody rb = spell.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * spellSpeed;
        }
    }
}