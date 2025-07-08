using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Comprehensive weapon system for Duck Game clone
/// Handles weapon switching, ammo management, and shooting mechanics
/// Based on authentic Duck Game mechanics
/// </summary>
public class WeaponSystem : MonoBehaviour
{
    [System.Serializable]
    public class Weapon
    {
        public string name;
        public WeaponType type;
        public int maxAmmo;
        public int currentAmmo;
        public float fireRate;
        public float reloadTime;
        public float damage;
        public float bulletSpeed;
        public bool isAutomatic;
        public AudioClip fireSound;
        public AudioClip reloadSound;
        public GameObject bulletPrefab;
        public Sprite weaponSprite;
        public bool isUnlocked = true;
    }
    
    public enum WeaponType
    {
        Pistol,
        Shotgun,
        MachineGun,
        Sniper,
        GrenadeLauncher,
        LaserGun
    }
    
    [Header("Weapon Configuration")]
    [SerializeField] private List<Weapon> availableWeapons = new List<Weapon>();
    [SerializeField] private int currentWeaponIndex = 0;
    [SerializeField] private bool canShoot = true;
    
    [Header("Shooting Settings")]
    [SerializeField] private Transform firePoint; // Where bullets spawn from
    [SerializeField] private float bulletLifetime = 3f;
    [SerializeField] private LayerMask bulletLayer = 1; // Default layer for bullets
    
    [Header("Audio")]
    [SerializeField] private AudioClip emptyGunSound;
    [SerializeField] private AudioClip weaponSwitchSound;
    
    // Private variables
    private float lastFireTime = 0f;
    private bool isReloading = false;
    private Coroutine reloadCoroutine;
    private Transform weaponHolder;
    private SpriteRenderer weaponSpriteRenderer;
    
    // Events
    public System.Action<Weapon> OnWeaponChanged;
    public System.Action<int, int> OnAmmoChanged; // current, max
    public System.Action OnReloadStart;
    public System.Action OnReloadComplete;
    
    void Start()
    {
        Debug.Log("=== WEAPON SYSTEM START ===");
        
        // Find fire point if not assigned
        if (firePoint == null)
        {
            Debug.Log("FirePoint not assigned, searching for it...");
            firePoint = transform.Find("FirePoint");
            if (firePoint == null)
            {
                Debug.LogError("FirePoint not found! Creating one...");
                // Create a fire point
                GameObject firePointObj = new GameObject("FirePoint");
                firePointObj.transform.SetParent(transform);
                firePointObj.transform.localPosition = new Vector3(1f, 0f, 0f); // Offset to the right
                firePoint = firePointObj.transform;
                Debug.Log("FirePoint created at: " + firePoint.position);
            }
        }
        else
        {
            Debug.Log("FirePoint found at: " + firePoint.position);
        }

        // Initialize weapons
        if (availableWeapons.Count == 0)
        {
            Debug.LogError("No weapons assigned to WeaponSystem!");
            return;
        }

        Debug.Log($"Initializing {availableWeapons.Count} weapons");
        foreach (Weapon weapon in availableWeapons)
        {
            if (weapon != null)
            {
                weapon.currentAmmo = weapon.maxAmmo;
                Debug.Log($"Initialized {weapon.name}: {weapon.currentAmmo}/{weapon.maxAmmo} ammo");
            }
        }

        currentWeaponIndex = 0;
        canShoot = true;
        isReloading = false;
        
        Debug.Log($"WeaponSystem initialized. Current weapon: {GetCurrentWeapon()?.name}");
    }
    
    void Update()
    {
        // Handle input
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Debug.Log("=== SHOOT INPUT DETECTED ===");
            if (canShoot && !isReloading)
            {
                Debug.Log("Input conditions met, calling Shoot()");
                Shoot();
            }
            else
            {
                Debug.Log($"Input conditions failed: canShoot={canShoot}, isReloading={isReloading}");
            }
        }

        // Handle weapon switching
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("=== WEAPON SWITCH INPUT DETECTED ===");
            NextWeapon();
        }

        // Handle reloading
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("=== RELOAD INPUT DETECTED ===");
            Reload();
        }

        // Update fire rate cooldown
        if (Time.time - lastFireTime < GetCurrentWeapon().fireRate)
        {
            canShoot = false;
        }
        else
        {
            canShoot = true;
        }
    }
    
    void LateUpdate()
    {
        // Flip weapon with duck
        if (weaponHolder != null)
        {
            float flip = transform.localScale.x;
            Vector3 newScale = new Vector3(flip, 1, 1);
            
            // Only update if the scale actually changed to avoid unnecessary updates
            if (weaponHolder.localScale != newScale)
            {
                weaponHolder.localScale = newScale;
            }
        }
    }
    
    /// <summary>
    /// Initializes the default weapons with authentic Duck Game stats
    /// </summary>
    private void InitializeWeapons()
    {
        if (availableWeapons.Count == 0)
        {
            // Create default weapons based on Duck Game
            availableWeapons.Add(new Weapon
            {
                name = "Pistol",
                type = WeaponType.Pistol,
                maxAmmo = 8,
                currentAmmo = 8,
                fireRate = 0.3f,
                reloadTime = 1.0f,
                damage = 25f,
                bulletSpeed = 15f,
                isAutomatic = false,
                fireSound = GetAudioClip("pistolFire"),
                reloadSound = GetAudioClip("click"),
                isUnlocked = true
            });
            
            availableWeapons.Add(new Weapon
            {
                name = "Shotgun",
                type = WeaponType.Shotgun,
                maxAmmo = 4,
                currentAmmo = 4,
                fireRate = 0.8f,
                reloadTime = 2.0f,
                damage = 50f,
                bulletSpeed = 12f,
                isAutomatic = false,
                fireSound = GetAudioClip("shotgunFire"),
                reloadSound = GetAudioClip("shotgunLoad"),
                isUnlocked = true
            });
            
            availableWeapons.Add(new Weapon
            {
                name = "Machine Gun",
                type = WeaponType.MachineGun,
                maxAmmo = 30,
                currentAmmo = 30,
                fireRate = 0.1f,
                reloadTime = 2.5f,
                damage = 15f,
                bulletSpeed = 18f,
                isAutomatic = true,
                fireSound = GetAudioClip("deepMachineGun"),
                reloadSound = GetAudioClip("click"),
                isUnlocked = true
            });
            
            availableWeapons.Add(new Weapon
            {
                name = "Sniper",
                type = WeaponType.Sniper,
                maxAmmo = 3,
                currentAmmo = 3,
                fireRate = 1.5f,
                reloadTime = 3.0f,
                damage = 100f,
                bulletSpeed = 25f,
                isAutomatic = false,
                fireSound = GetAudioClip("magnum"),
                reloadSound = GetAudioClip("click"),
                isUnlocked = true
            });
        }
    }
    
    /// <summary>
    /// Sets up the fire point for shooting
    /// </summary>
    private void SetupFirePoint()
    {
        if (firePoint == null)
        {
            // Create fire point as child of the duck
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(transform);
            firePointObj.transform.localPosition = new Vector3(0.5f, 0.2f, 0); // Slightly to the right and up
            firePoint = firePointObj.transform;
        }
    }
    
    /// <summary>
    /// Handles weapon switching input
    /// </summary>
    private void HandleWeaponInput()
    {
        // Number keys 1-9 for weapon switching
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && i < availableWeapons.Count)
            {
                SwitchWeapon(i);
            }
        }
        
        // Mouse wheel for weapon switching
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0)
        {
            NextWeapon();
        }
        else if (scroll < 0)
        {
            PreviousWeapon();
        }
        
        // R key for reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }
    
    /// <summary>
    /// Handles shooting input
    /// </summary>
    private void HandleShootingInput()
    {
        if (!canShoot || isReloading) 
        {
            return;
        }

        Weapon currentWeapon = GetCurrentWeapon();
        if (currentWeapon == null) 
        {
            Debug.LogError("Current weapon is null!");
            return;
        }

        // Check if weapon is automatic or single shot
        bool shouldShoot = currentWeapon.isAutomatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");

        if (shouldShoot && Time.time >= lastFireTime + currentWeapon.fireRate)
        {
            Shoot();
        }
    }
    
    /// <summary>
    /// Switches to the specified weapon
    /// </summary>
    public void SwitchWeapon(int weaponIndex)
    {
        if (weaponIndex < 0 || weaponIndex >= availableWeapons.Count) return;
        if (weaponIndex == currentWeaponIndex) return;
        
        currentWeaponIndex = weaponIndex;
        
        // Refill ammo for testing
        var weapon = GetCurrentWeapon();
        if (weapon != null) weapon.currentAmmo = weapon.maxAmmo;

        // Play switch sound
        if (AudioManager.Instance != null && weaponSwitchSound != null)
        {
            AudioManager.Instance.PlaySound(weaponSwitchSound);
        }
        
        // Trigger event
        OnWeaponChanged?.Invoke(GetCurrentWeapon());
        UpdateWeaponVisual();
    }
    
    /// <summary>
    /// Switches to the next weapon
    /// </summary>
    public void NextWeapon()
    {
        int nextIndex = (currentWeaponIndex + 1) % availableWeapons.Count;
        SwitchWeapon(nextIndex);
    }
    
    /// <summary>
    /// Switches to the previous weapon
    /// </summary>
    public void PreviousWeapon()
    {
        int prevIndex = currentWeaponIndex - 1;
        if (prevIndex < 0) prevIndex = availableWeapons.Count - 1;
        SwitchWeapon(prevIndex);
    }
    
    /// <summary>
    /// Shoots the current weapon
    /// </summary>
    public void Shoot()
    {
        Debug.Log("=== SHOOT() METHOD CALLED ===");
        
        Weapon weapon = GetCurrentWeapon();
        if (weapon == null)
        {
            Debug.LogError("Shoot() called but currentWeapon is null!");
            return;
        }

        Debug.Log($"Shooting weapon: {weapon.name}, ammo: {weapon.currentAmmo}/{weapon.maxAmmo}");

        // Check ammo
        if (weapon.currentAmmo <= 0)
        {
            Debug.Log("No ammo! Playing empty sound.");
            PlayEmptySound();
            return;
        }

        // Reduce ammo
        weapon.currentAmmo--;
        OnAmmoChanged?.Invoke(weapon.currentAmmo, weapon.maxAmmo);

        // Update fire time
        lastFireTime = Time.time;

        // Play fire sound
        if (AudioManager.Instance != null && weapon.fireSound != null)
        {
            AudioManager.Instance.PlaySound(weapon.fireSound);
            Debug.Log("Fire sound played");
        }
        else
        {
            Debug.Log("No fire sound: AudioManager=" + (AudioManager.Instance != null) + ", fireSound=" + (weapon.fireSound != null));
        }

        // Create bullet
        Debug.Log("Creating bullet...");
        CreateBullet(weapon);
        
        Debug.Log($"Shot completed! Ammo remaining: {weapon.currentAmmo}");
    }
    
    /// <summary>
    /// Creates a bullet based on weapon type
    /// </summary>
    private void CreateBullet(Weapon weapon)
    {
        Debug.Log($"CreateBullet called for {weapon.name}, type: {weapon.type}");
        
        if (firePoint == null) 
        {
            Debug.LogError("FirePoint is null! Cannot create bullet.");
            return;
        }
        
        Debug.Log($"FirePoint position: {firePoint.position}");
        
        switch (weapon.type)
        {
            case WeaponType.Pistol:
            case WeaponType.Sniper:
                Debug.Log("Creating single bullet");
                CreateSingleBullet(weapon);
                break;
                
            case WeaponType.Shotgun:
                Debug.Log("Creating shotgun blast");
                CreateShotgunBlast(weapon);
                break;
                
            case WeaponType.MachineGun:
                Debug.Log("Creating machine gun bullet");
                CreateSingleBullet(weapon);
                break;
                
            default:
                Debug.Log("Creating default single bullet");
                CreateSingleBullet(weapon);
                break;
        }
    }
    
    /// <summary>
    /// Creates a single bullet
    /// </summary>
    private void CreateSingleBullet(Weapon weapon)
    {
        Debug.Log("CreateSingleBullet called");
        GameObject bullet = CreateBulletObject(weapon);
        if (bullet != null)
        {
            Debug.Log($"Single bullet created: {bullet.name}");
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                // Shoot in the direction the duck is facing
                Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
                bulletRb.velocity = direction * weapon.bulletSpeed;
                Debug.Log($"Bullet velocity set: {bulletRb.velocity}, direction: {direction}, speed: {weapon.bulletSpeed}");
            }
            else
            {
                Debug.LogError("Bullet Rigidbody2D is null!");
            }
        }
        else
        {
            Debug.LogError("CreateBulletObject returned null!");
        }
    }
    
    /// <summary>
    /// Creates a shotgun blast (multiple bullets)
    /// </summary>
    private void CreateShotgunBlast(Weapon weapon)
    {
        int pelletCount = 5;
        float spreadAngle = 30f;
        
        for (int i = 0; i < pelletCount; i++)
        {
            GameObject bullet = CreateBulletObject(weapon);
            if (bullet != null)
            {
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                if (bulletRb != null)
                {
                    // Calculate spread
                    float angle = (i - pelletCount / 2f) * (spreadAngle / pelletCount);
                    Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
                    direction = Quaternion.Euler(0, 0, angle) * direction;
                    
                    bulletRb.velocity = direction * weapon.bulletSpeed;
                }
            }
        }
    }
    
    /// <summary>
    /// Creates a bullet GameObject
    /// </summary>
    private GameObject CreateBulletObject(Weapon weapon)
    {
        Debug.Log($"CreateBulletObject called for {weapon.name}");
        
        GameObject bullet;
        
        if (weapon.bulletPrefab != null)
        {
            Debug.Log("Using bullet prefab");
            bullet = Instantiate(weapon.bulletPrefab, firePoint.position, firePoint.rotation);
        }
        else
        {
            Debug.Log("Creating basic bullet (no prefab)");
            // Create a basic bullet
            bullet = new GameObject($"Bullet_{weapon.name}");
            bullet.transform.position = firePoint.position;
            
            // Add sprite renderer if missing
            SpriteRenderer spriteRenderer = bullet.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateBasicBulletSprite();
            spriteRenderer.color = Color.yellow;
            spriteRenderer.sortingOrder = 10; // Make sure bullets are visible
            
            // Add collider
            CircleCollider2D collider = bullet.AddComponent<CircleCollider2D>();
            collider.radius = 0.1f;
            collider.isTrigger = true; // Make it a trigger for better collision detection
            
            // Add rigidbody
            Rigidbody2D rb = bullet.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Better collision detection
            
            // Add bullet script
            bullet.AddComponent<Bullet>().Initialize(weapon.damage, bulletLifetime);
            Debug.Log("Basic bullet components added successfully");
        }
        
        // Set layer - use a simple approach
        bullet.layer = 8; // Use layer 8 for bullets (make sure this layer exists)
        Debug.Log($"Bullet created at layer {bullet.layer}, position: {bullet.transform.position}");
        
        return bullet;
    }
    
    /// <summary>
    /// Creates a basic bullet sprite
    /// </summary>
    private Sprite CreateBasicBulletSprite()
    {
        Texture2D texture = new Texture2D(16, 16);
        Color[] pixels = new Color[16 * 16];
        
        // Create a more visible bullet sprite
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                // Create a circular bullet
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(8, 8));
                if (distance < 6)
                {
                    pixels[y * 16 + x] = Color.white;
                }
                else
                {
                    pixels[y * 16 + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f));
    }
    
    /// <summary>
    /// Reloads the current weapon
    /// </summary>
    public void Reload()
    {
        Weapon weapon = GetCurrentWeapon();
        if (weapon == null || isReloading) return;
        
        if (weapon.currentAmmo >= weapon.maxAmmo)
        {
            return;
        }
        
        // Start reload coroutine
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
        }
        
        reloadCoroutine = StartCoroutine(ReloadCoroutine(weapon));
    }
    
    /// <summary>
    /// Coroutine for reloading
    /// </summary>
    private IEnumerator ReloadCoroutine(Weapon weapon)
    {
        isReloading = true;
        OnReloadStart?.Invoke();
        
        // Play reload sound
        if (AudioManager.Instance != null && weapon.reloadSound != null)
        {
            AudioManager.Instance.PlaySound(weapon.reloadSound);
        }
        
        // Wait for reload time
        yield return new WaitForSeconds(weapon.reloadTime);
        
        // Refill ammo
        weapon.currentAmmo = weapon.maxAmmo;
        OnAmmoChanged?.Invoke(weapon.currentAmmo, weapon.maxAmmo);
        
        isReloading = false;
        OnReloadComplete?.Invoke();
    }
    
    /// <summary>
    /// Plays empty gun sound
    /// </summary>
    private void PlayEmptySound()
    {
        if (AudioManager.Instance != null && emptyGunSound != null)
        {
            AudioManager.Instance.PlaySound(emptyGunSound);
        }
    }
    
    /// <summary>
    /// Gets the current weapon
    /// </summary>
    public Weapon GetCurrentWeapon()
    {
        Debug.Log($"GetCurrentWeapon: availableWeapons.Count={availableWeapons.Count}, currentWeaponIndex={currentWeaponIndex}");
        
        if (availableWeapons.Count == 0 || currentWeaponIndex >= availableWeapons.Count)
        {
            Debug.LogError("GetCurrentWeapon returning null!");
            return null;
        }
        
        Weapon weapon = availableWeapons[currentWeaponIndex];
        Debug.Log($"GetCurrentWeapon returning: {weapon?.name}");
        return weapon;
    }
    
    /// <summary>
    /// Updates the weapon visual based on the current weapon
    /// </summary>
    private void UpdateWeaponVisual()
    {
        if (weaponSpriteRenderer == null) return;
        var weapon = GetCurrentWeapon();
        if (weapon != null && weapon.weaponSprite != null)
        {
            weaponSpriteRenderer.sprite = weapon.weaponSprite;
            weaponSpriteRenderer.enabled = true;
        }
        else
        {
            weaponSpriteRenderer.sprite = null;
            weaponSpriteRenderer.enabled = false;
        }
    }
    
    /// <summary>
    /// Gets audio clip by name from AudioManager
    /// </summary>
    private AudioClip GetAudioClip(string clipName)
    {
        // This will be handled by AudioManager
        return null; // AudioManager will find the clip by name
    }
    
    /// <summary>
    /// Adds ammo to the current wepon
    /// </summary>
    public void AddAmmo(int amount)
    {
        Weapon weapon = GetCurrentWeapon();
        if (weapon != null)
        {
            weapon.currentAmmo = Mathf.Min(weapon.currentAmmo + amount, weapon.maxAmmo);
            OnAmmoChanged?.Invoke(weapon.currentAmmo, weapon.maxAmmo);
        }
    }
    
    /// <summary>
    /// Unlocks a weapon
    /// </summary>
    public void UnlockWeapon(WeaponType weaponType)
    {
        foreach (Weapon weapon in availableWeapons)
        {
            if (weapon.type == weaponType)
            {
                weapon.isUnlocked = true;
                break;
            }
        }
    }
    
    /// <summary>
    /// Test method to shoot without input (for debugging)
    /// </summary>
    [ContextMenu("Test Shoot")]
    public void TestShoot()
    {
        Debug.Log("=== TEST SHOOT CALLED ===");
        if (canShoot && !isReloading)
        {
            Debug.Log("Test shoot conditions met, calling Shoot()");
            Shoot();
        }
        else
        {
            Debug.Log($"Test shoot failed: canShoot={canShoot}, isReloading={isReloading}");
        }
    }
    
    /// <summary>
    /// Test method to switch to next weapon (for debugging)
    /// </summary>
    [ContextMenu("Test Next Weapon")]
    public void TestNextWeapon()
    {
        Debug.Log("=== TEST NEXT WEAPON CALLED ===");
        NextWeapon();
    }
} 