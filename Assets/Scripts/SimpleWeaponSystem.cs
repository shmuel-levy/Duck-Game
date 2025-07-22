using UnityEngine;

public class SimpleWeaponSystem : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireForce = 20f;

    void Update()
    {
        // Shoot with left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Bullet prefab or fire point not assigned!");
            return;
        }
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Use Duck's facing direction to determine bullet direction
            float direction = transform.localScale.x < 0 ? -1f : 1f;
            rb.AddForce(Vector2.right * fireForce * direction, ForceMode2D.Impulse);
        }
    }
} 