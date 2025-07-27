using UnityEngine;

public class SimpleWeaponSystem : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireForce = 20f;
    
    // Reference to weapon switcher to get current weapon
    private SimpleWeaponSwitcher weaponSwitcher;

    void Start()
    {
        // Get reference to weapon switcher
        weaponSwitcher = GetComponent<SimpleWeaponSwitcher>();
    }

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
        
        // Play weapon sound based on current weapon
        PlayWeaponSound();
        
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Use Duck's facing direction to determine bullet direction
            float direction = transform.localScale.x < 0 ? -1f : 1f;
            rb.AddForce(Vector2.right * fireForce * direction, ForceMode2D.Impulse);
        }
    }
    
    void PlayWeaponSound()
    {
        if (AudioManager.Instance == null) return;
        
        // Get current weapon index from weapon switcher
        int currentWeapon = 0;
        if (weaponSwitcher != null)
        {
            // Access the current weapon index (we'll need to make this public)
            currentWeapon = weaponSwitcher.GetCurrentWeaponIndex();
        }
        
        // Play appropriate sound based on weapon index
        switch (currentWeapon)
        {
            case 0: // Pistol
                AudioManager.Instance.PlaySound("pistolFire");
                break;
            case 1: // Shotgun
                AudioManager.Instance.PlaySound("shotgunFire");
                break;
            case 2: // Machine Gun
                AudioManager.Instance.PlaySound("machineGunFire");
                break;
            case 3: // Sniper
                AudioManager.Instance.PlaySound("sniperFire");
                break;
            default:
                AudioManager.Instance.PlaySound("pistolFire");
                break;
        }
    }
} 