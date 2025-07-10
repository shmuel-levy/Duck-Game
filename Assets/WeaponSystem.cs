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
    
    [Header("Weapon Sprites")]
    [SerializeField] private Sprite pistolSprite;
    [SerializeField] private Sprite shotgunSprite;
    [SerializeField] private Sprite machinegunSprite;
    [SerializeField] private Sprite sniperSprite;
    
    [Header("Weapon Positioning")]
    [SerializeField] private float weaponScale = 2f;
    [SerializeField] private float weaponOffsetX = 0.5f;
    [SerializeField] private float weaponOffsetY = 0.2f;
    [SerializeField] private float firePointOffsetX = 0.8f;
    [SerializeField] private float firePointOffsetY = 0.2f;
    
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
        // Initialize weapon holder and sprite renderer
        InitializeWeaponVisuals();
        
        // Find fire point if not assigned
        if (firePoint == null)
        {
            firePoint = transform.Find("FirePoint");
            if (firePoint == null)
            {
                // Create a fire point
                GameObject firePointObj = new GameObject("FirePoint");
                firePointObj.transform.SetParent(transform);
                firePointObj.transform.localPosition = new Vector3(1f, 0f, 0f); // Offset to the right
                firePoint = firePointObj.transform;
            }
        }

        // Force weapon initialization
        if (availableWeapons.Count == 0)
        {
            ManualInitializeWeapons();
        }
        else
        {
            // Ensure weapons have proper fire rates for testing
            foreach (var weapon in availableWeapons)
            {
                if (weapon.fireRate > 0.5f) // If fire rate is too slow, make it faster for testing
                {
                    weapon.fireRate = 0.2f; // Faster fire rate for testing
                }
            }
        }

        currentWeaponIndex = 0;
        canShoot = true;
        isReloading = false;
        
        // Update weapon visual after initialization
        UpdateWeaponVisual();
        
        // Check if weapons need audio clips assigned
        StartCoroutine(CheckAndAssignAudioClips());
    }
    
    /// <summary>
    /// Initializes the weapon holder and sprite renderer for weapon visibility
    /// </summary>
    private void InitializeWeaponVisuals()
    {
        // Create weapon holder if it doesn't exist
        if (weaponHolder == null)
        {
            weaponHolder = transform.Find("WeaponHolder");
            if (weaponHolder == null)
            {
                GameObject weaponHolderObj = new GameObject("WeaponHolder");
                weaponHolderObj.transform.SetParent(transform);
                weaponHolderObj.transform.localPosition = new Vector3(0.5f, 0.2f, 0f); // Slightly to the right and up
                weaponHolder = weaponHolderObj.transform;
            }
        }
        
        // Get or create sprite renderer
        if (weaponSpriteRenderer == null)
        {
            weaponSpriteRenderer = weaponHolder.GetComponent<SpriteRenderer>();
            if (weaponSpriteRenderer == null)
            {
                weaponSpriteRenderer = weaponHolder.gameObject.AddComponent<SpriteRenderer>();
                weaponSpriteRenderer.sortingOrder = 5; // Make sure weapon is visible above duck
            }
        }
    }
    
    void Update()
    {
        // Handle input - Left Mouse Button for shooting
        if (Input.GetMouseButtonDown(0))
        {
            if (canShoot && !isReloading)
            {
                Shoot();
            }
        }

        // Handle weapon switching
        if (Input.GetKeyDown(KeyCode.Q))
        {
            NextWeapon();
        }

        // Handle reloading
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        // Update fire rate cooldown
        Weapon currentWeapon = GetCurrentWeapon();
        if (currentWeapon != null)
        {
            canShoot = Time.time - lastFireTime >= currentWeapon.fireRate;
        }
        else
        {
            canShoot = false;
        }
    }
    
    void LateUpdate()
    {
        // Flip weapon with duck and adjust position
        if (weaponHolder != null)
        {
            float duckFlip = transform.localScale.x;
            
            // Flip the weapon scale to match duck direction
            Vector3 newScale = new Vector3(duckFlip * weaponScale, weaponScale, 1f);
            
            // Adjust weapon position based on duck direction
            Vector3 newPosition = new Vector3(duckFlip * weaponOffsetX, weaponOffsetY, 0f);
            
            // Only update if something actually changed to avoid unnecessary updates
            if (weaponHolder.localScale != newScale || weaponHolder.localPosition != newPosition)
            {
                weaponHolder.localScale = newScale;
                weaponHolder.localPosition = newPosition;
                
                // Also update fire point position
                if (firePoint != null)
                {
                    firePoint.localPosition = new Vector3(duckFlip * firePointOffsetX, firePointOffsetY, 0f);
                }
            }
        }
    }
    

    
    /// <summary>
    /// Loads weapon sprite from the assigned sprite fields
    /// </summary>
    private Sprite LoadWeaponSprite(string spriteName)
    {
        Sprite sprite = null;
        
        // Use the assigned sprite fields
        switch (spriteName.ToLower())
        {
            case "pistol":
                sprite = pistolSprite;
                break;
            case "shotgun":
                sprite = shotgunSprite;
                break;
            case "machinegun":
                sprite = machinegunSprite;
                break;
            case "sniper":
                sprite = sniperSprite;
                break;
        }
        
        if (sprite == null)
        {
            Debug.LogWarning($"Weapon sprite for {spriteName} not assigned in inspector. Creating procedural sprite instead.");
            // Fallback to procedural sprite if not assigned
            WeaponType weaponType = GetWeaponTypeFromSpriteName(spriteName);
            sprite = CreateWeaponSprite(weaponType);
        }
        
        return sprite;
    }
    
    /// <summary>
    /// Maps sprite names to weapon types
    /// </summary>
    private WeaponType GetWeaponTypeFromSpriteName(string spriteName)
    {
        switch (spriteName.ToLower())
        {
            case "pistol":
                return WeaponType.Pistol;
            case "shotgun":
                return WeaponType.Shotgun;
            case "machinegun":
                return WeaponType.MachineGun;
            case "sniper":
                return WeaponType.Sniper;
            default:
                return WeaponType.Pistol;
        }
    }
    
    /// <summary>
    /// Maps weapon types to sprite names
    /// </summary>
    private string GetSpriteNameForWeaponType(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Pistol:
                return "pistol";
            case WeaponType.Shotgun:
                return "shotgun";
            case WeaponType.MachineGun:
                return "machinegun";
            case WeaponType.Sniper:
                return "sniper";
            default:
                return "pistol";
        }
    }
    
    /// <summary>
    /// Creates a weapon-specific sprite
    /// </summary>
    private Sprite CreateWeaponSprite(WeaponType weaponType)
    {
        // Create a much larger, more visible weapon sprite
        Texture2D texture = new Texture2D(128, 64);
        Color[] pixels = new Color[128 * 64];
        
        // Get weapon-specific colors
        Color barrelColor, handleColor, highlightColor;
        
        switch (weaponType)
        {
            case WeaponType.Pistol:
                barrelColor = new Color(0.3f, 0.3f, 0.3f); // Gray
                handleColor = new Color(0.4f, 0.2f, 0.1f); // Brown
                highlightColor = new Color(1f, 1f, 0f); // Yellow
                break;
            case WeaponType.Shotgun:
                barrelColor = new Color(0.2f, 0.2f, 0.2f); // Dark gray
                handleColor = new Color(0.3f, 0.15f, 0.05f); // Dark brown
                highlightColor = new Color(1f, 0.5f, 0f); // Orange
                break;
            case WeaponType.MachineGun:
                barrelColor = new Color(0.1f, 0.1f, 0.1f); // Very dark gray
                handleColor = new Color(0.2f, 0.1f, 0.05f); // Very dark brown
                highlightColor = new Color(1f, 0f, 0f); // Red
                break;
            case WeaponType.Sniper:
                barrelColor = new Color(0.4f, 0.4f, 0.4f); // Light gray
                handleColor = new Color(0.5f, 0.25f, 0.1f); // Light brown
                highlightColor = new Color(0f, 1f, 1f); // Cyan
                break;
            default:
                barrelColor = new Color(0.3f, 0.3f, 0.3f);
                handleColor = new Color(0.4f, 0.2f, 0.1f);
                highlightColor = new Color(1f, 1f, 1f);
                break;
        }
        
        // Create a simple gun shape that's much more visible
        for (int x = 0; x < 128; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                // Gun barrel (long rectangle) - make it very visible
                if (x >= 32 && x <= 112 && y >= 24 && y <= 40)
                {
                    pixels[y * 128 + x] = barrelColor;
                }
                // Gun handle (small rectangle) - make it very visible
                else if (x >= 16 && x <= 48 && y >= 8 && y <= 32)
                {
                    pixels[y * 128 + x] = handleColor;
                }
                // Add bright highlights to make it very visible
                else if (x >= 36 && x <= 108 && y >= 28 && y <= 36)
                {
                    pixels[y * 128 + x] = highlightColor;
                }
                // Add some metallic highlights
                else if (x >= 40 && x <= 104 && y >= 30 && y <= 34)
                {
                    pixels[y * 128 + x] = new Color(0.9f, 0.9f, 0.9f); // Light gray
                }
                else
                {
                    pixels[y * 128 + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        // Create sprite with proper pivot point
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 128, 64), new Vector2(0.2f, 0.5f));
        
        return sprite;
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
    /// Switches to the specified weapon
    /// </summary>
    public void SwitchWeapon(int weaponIndex)
    {
        if (weaponIndex < 0 || weaponIndex >= availableWeapons.Count)
        {
            return;
        }
        
        if (weaponIndex == currentWeaponIndex)
        {
            return;
        }
        
        currentWeaponIndex = weaponIndex;
        
        // Get the weapon and ensure it has proper data
        var weapon = GetCurrentWeapon();
        if (weapon != null)
        {
            // Refill ammo for testing
            weapon.currentAmmo = weapon.maxAmmo;
            
            // Ensure weapon has a name
            if (string.IsNullOrEmpty(weapon.name))
            {
                weapon.name = $"Weapon_{weaponIndex}";
            }
            
            // Ensure weapon has a sprite
            if (weapon.weaponSprite == null)
            {
                // Use assigned sprite fields directly
                switch (weapon.type)
                {
                    case WeaponType.Pistol:
                        weapon.weaponSprite = pistolSprite != null ? pistolSprite : CreateWeaponSprite(WeaponType.Pistol);
                        break;
                    case WeaponType.Shotgun:
                        weapon.weaponSprite = shotgunSprite != null ? shotgunSprite : CreateWeaponSprite(WeaponType.Shotgun);
                        break;
                    case WeaponType.MachineGun:
                        weapon.weaponSprite = machinegunSprite != null ? machinegunSprite : CreateWeaponSprite(WeaponType.MachineGun);
                        break;
                    case WeaponType.Sniper:
                        weapon.weaponSprite = sniperSprite != null ? sniperSprite : CreateWeaponSprite(WeaponType.Sniper);
                        break;
                    default:
                        weapon.weaponSprite = CreateWeaponSprite(weapon.type);
                        break;
                }
            }
        }

        // Play switch sound
        if (AudioManager.Instance != null && weaponSwitchSound != null)
        {
            AudioManager.Instance.PlaySound(weaponSwitchSound);
        }
        
        // Trigger event
        OnWeaponChanged?.Invoke(GetCurrentWeapon());
        
        // Update weapon visual immediately
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
        Weapon weapon = GetCurrentWeapon();
        if (weapon == null)
        {
            return;
        }

        // Check ammo
        if (weapon.currentAmmo <= 0)
        {
            PlayEmptySound();
            return;
        }

        // Check fire point
        if (firePoint == null)
        {
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
        }

        // Create bullet
        CreateBullet(weapon);
    }
    
    /// <summary>
    /// Creates a bullet based on weapon type
    /// </summary>
    private void CreateBullet(Weapon weapon)
    {
        if (firePoint == null) 
        {
            return;
        }
        
        switch (weapon.type)
        {
            case WeaponType.Pistol:
            case WeaponType.Sniper:
                CreateSingleBullet(weapon);
                break;
                
            case WeaponType.Shotgun:
                CreateShotgunBlast(weapon);
                break;
                
            case WeaponType.MachineGun:
                CreateSingleBullet(weapon);
                break;
                
            default:
                CreateSingleBullet(weapon);
                break;
        }
    }
    
    /// <summary>
    /// Creates a single bullet
    /// </summary>
    private void CreateSingleBullet(Weapon weapon)
    {
        GameObject bullet = CreateBulletObject(weapon);
        if (bullet != null)
        {
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                // Shoot in the direction the duck is facing
                Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
                bulletRb.velocity = direction * weapon.bulletSpeed;
            }
            else
            {
                // Bullet Rigidbody2D is null
            }
        }
        else
        {
            // CreateBulletObject returned null
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
        GameObject bullet;
        
        if (weapon.bulletPrefab != null)
        {
            bullet = Instantiate(weapon.bulletPrefab, firePoint.position, firePoint.rotation);
        }
        else
        {
            // Create a Duck Game-style bullet
            bullet = new GameObject($"Bullet_{weapon.name}");
            bullet.transform.position = firePoint.position;
            
            // Add sprite renderer
            SpriteRenderer spriteRenderer = bullet.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateDuckGameBulletSprite(weapon.type);
            spriteRenderer.sortingOrder = 20; // Higher sorting order to ensure visibility
            
            // Set bullet color based on weapon type (Duck Game style)
            Color bulletColor = GetBulletColor(weapon.type);
            spriteRenderer.color = bulletColor;
            
            // Scale the bullet (make it larger and more visible)
            bullet.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            
            // Add collider
            CircleCollider2D collider = bullet.AddComponent<CircleCollider2D>();
            collider.radius = 0.2f;
            collider.isTrigger = true;
            
            // Add rigidbody
            Rigidbody2D rb = bullet.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            
            // Ensure bullet is active
            bullet.SetActive(true);
            
            // Add bullet script
            bullet.AddComponent<Bullet>().Initialize(weapon.damage, bulletLifetime);
        }
        
        // Set layer
        bullet.layer = 8;
        
        return bullet;
    }
    
    /// <summary>
    /// Creates a retro-style bullet sprite
    /// </summary>
    private Sprite CreateDuckGameBulletSprite(WeaponType weaponType)
    {
        // Create a larger, more visible retro bullet sprite
        Texture2D texture = new Texture2D(24, 24);
        Color[] pixels = new Color[24 * 24];
        
        // Create a retro-style bullet with bright colors and glow effect
        for (int x = 0; x < 24; x++)
        {
            for (int y = 0; y < 24; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(12, 12));
                
                if (distance < 4)
                {
                    // Bright white center
                    pixels[y * 24 + x] = Color.white;
                }
                else if (distance < 8)
                {
                    // Bright yellow core
                    pixels[y * 24 + x] = new Color(1f, 1f, 0f, 1f); // Bright yellow
                }
                else if (distance < 10)
                {
                    // Orange glow
                    pixels[y * 24 + x] = new Color(1f, 0.7f, 0f, 0.8f); // Bright orange
                }
                else if (distance < 12)
                {
                    // Red glow
                    pixels[y * 24 + x] = new Color(1f, 0.3f, 0f, 0.6f); // Bright red-orange
                }
                else
                {
                    pixels[y * 24 + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 24, 24), new Vector2(0.5f, 0.5f));
    }
    
    /// <summary>
    /// Gets the bullet color for each weapon type (retro style)
    /// </summary>
    private Color GetBulletColor(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Pistol:
                return new Color(1f, 1f, 0f, 1f); // Bright yellow
            case WeaponType.Shotgun:
                return new Color(1f, 0.8f, 0f, 1f); // Bright orange
            case WeaponType.MachineGun:
                return new Color(1f, 0.2f, 0.2f, 1f); // Bright red
            case WeaponType.Sniper:
                return new Color(0f, 1f, 1f, 1f); // Bright cyan
            default:
                return new Color(1f, 1f, 0f, 1f); // Bright yellow
        }
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
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound("emptyGun");
        }
    }
    
    /// <summary>
    /// Gets the current weapon
    /// </summary>
    public Weapon GetCurrentWeapon()
    {
        if (availableWeapons.Count == 0 || currentWeaponIndex >= availableWeapons.Count)
        {
            return null;
        }
        
        Weapon weapon = availableWeapons[currentWeaponIndex];
        return weapon;
    }
    
    /// <summary>
    /// Updates the weapon visual based on the current weapon
    /// </summary>
    private void UpdateWeaponVisual()
    {
        if (weaponSpriteRenderer == null)
        {
            InitializeWeaponVisuals(); // Try to initialize again
            if (weaponSpriteRenderer == null)
            {
                Debug.LogError("Failed to initialize WeaponSpriteRenderer!");
                return;
            }
        }
        
        var weapon = GetCurrentWeapon();
        if (weapon != null)
        {
            // Always ensure we have a sprite
            if (weapon.weaponSprite == null)
            {
                // Use assigned sprite fields directly
                switch (weapon.type)
                {
                    case WeaponType.Pistol:
                        weapon.weaponSprite = pistolSprite != null ? pistolSprite : CreateWeaponSprite(WeaponType.Pistol);
                        break;
                    case WeaponType.Shotgun:
                        weapon.weaponSprite = shotgunSprite != null ? shotgunSprite : CreateWeaponSprite(WeaponType.Shotgun);
                        break;
                    case WeaponType.MachineGun:
                        weapon.weaponSprite = machinegunSprite != null ? machinegunSprite : CreateWeaponSprite(WeaponType.MachineGun);
                        break;
                    case WeaponType.Sniper:
                        weapon.weaponSprite = sniperSprite != null ? sniperSprite : CreateWeaponSprite(WeaponType.Sniper);
                        break;
                    default:
                        weapon.weaponSprite = CreateWeaponSprite(weapon.type);
                        break;
                }
            }
            
            weaponSpriteRenderer.sprite = weapon.weaponSprite;
            weaponSpriteRenderer.enabled = true;
            weaponSpriteRenderer.sortingOrder = 5; // Ensure it's visible above duck
            
            // Don't override scale here - let LateUpdate handle flipping
            // weaponHolder.localScale = new Vector3(2f, 2f, 1f); // Make it 2x larger
        }
        else
        {
            Debug.LogWarning("Current weapon is null!");
            weaponSpriteRenderer.sprite = null;
            weaponSpriteRenderer.enabled = false;
        }
    }
    
    /// <summary>
    /// Gets audio clip by name from AudioManager
    /// </summary>
    private AudioClip GetAudioClip(string clipName)
    {
        if (AudioManager.Instance != null)
        {
            AudioClip clip = AudioManager.Instance.GetAudioClip(clipName);
            return clip;
        }
        else
        {
            Debug.LogError("AudioManager.Instance is null!");
        }
        return null;
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
    /// Manually create FirePoint
    /// </summary>
    [ContextMenu("Create FirePoint")]
    public void CreateFirePoint()
    {
        // Destroy existing fire point if it exists
        if (firePoint != null)
        {
            DestroyImmediate(firePoint.gameObject);
            firePoint = null;
        }
        
        // Create new fire point
        GameObject firePointObj = new GameObject("FirePoint");
        firePointObj.transform.SetParent(transform);
        firePointObj.transform.localPosition = new Vector3(1f, 0f, 0f);
        firePoint = firePointObj.transform;
    }
    
    /// <summary>
    /// Manually initialize weapons
    /// </summary>
    [ContextMenu("Initialize Weapons")]
    public void ManualInitializeWeapons()
    {
        // Store current firepoint position
        Vector3 firePointPosition = Vector3.zero;
        if (firePoint != null)
        {
            firePointPosition = firePoint.position;
        }
        
        // Clear existing weapons
        availableWeapons.Clear();
        
        // Create weapons manually with explicit properties
        Weapon pistol = new Weapon();
        pistol.name = "Pistol";
        pistol.type = WeaponType.Pistol;
        pistol.maxAmmo = 8;
        pistol.currentAmmo = 8;
        pistol.fireRate = 0.3f;
        pistol.reloadTime = 1.0f;
        pistol.damage = 25f;
        pistol.bulletSpeed = 15f;
        pistol.isAutomatic = false;
        pistol.fireSound = GetAudioClip("pistolFire");
        pistol.reloadSound = GetAudioClip("weaponReload");
        pistol.weaponSprite = pistolSprite != null ? pistolSprite : CreateWeaponSprite(WeaponType.Pistol);
        pistol.isUnlocked = true;
        availableWeapons.Add(pistol);
        
        Weapon shotgun = new Weapon();
        shotgun.name = "Shotgun";
        shotgun.type = WeaponType.Shotgun;
        shotgun.maxAmmo = 4;
        shotgun.currentAmmo = 4;
        shotgun.fireRate = 0.8f;
        shotgun.reloadTime = 2.0f;
        shotgun.damage = 50f;
        shotgun.bulletSpeed = 12f;
        shotgun.isAutomatic = false;
        shotgun.fireSound = GetAudioClip("shotgunFire");
        shotgun.reloadSound = GetAudioClip("weaponReload");
        shotgun.weaponSprite = shotgunSprite != null ? shotgunSprite : CreateWeaponSprite(WeaponType.Shotgun);
        shotgun.isUnlocked = true;
        availableWeapons.Add(shotgun);
        
        Weapon machinegun = new Weapon();
        machinegun.name = "Machine Gun";
        machinegun.type = WeaponType.MachineGun;
        machinegun.maxAmmo = 30;
        machinegun.currentAmmo = 30;
        machinegun.fireRate = 0.1f;
        machinegun.reloadTime = 2.5f;
        machinegun.damage = 15f;
        machinegun.bulletSpeed = 18f;
        machinegun.isAutomatic = true;
        machinegun.fireSound = GetAudioClip("machineGunFire");
        machinegun.reloadSound = GetAudioClip("weaponReload");
        machinegun.weaponSprite = machinegunSprite != null ? machinegunSprite : CreateWeaponSprite(WeaponType.MachineGun);
        machinegun.isUnlocked = true;
        availableWeapons.Add(machinegun);
        
        Weapon sniper = new Weapon();
        sniper.name = "Sniper";
        sniper.type = WeaponType.Sniper;
        sniper.maxAmmo = 3;
        sniper.currentAmmo = 3;
        sniper.fireRate = 1.5f;
        sniper.reloadTime = 3.0f;
        sniper.damage = 100f;
        sniper.bulletSpeed = 25f;
        sniper.isAutomatic = false;
        sniper.fireSound = GetAudioClip("sniperFire");
        sniper.reloadSound = GetAudioClip("weaponReload");
        sniper.weaponSprite = sniperSprite != null ? sniperSprite : CreateWeaponSprite(WeaponType.Sniper);
        sniper.isUnlocked = true;
        availableWeapons.Add(sniper);
        
        // Set current weapon index and ensure all weapons have proper names
        currentWeaponIndex = 0;
        
        // Ensure all weapons have proper names
        for (int i = 0; i < availableWeapons.Count; i++)
        {
            var weapon = availableWeapons[i];
            if (string.IsNullOrEmpty(weapon.name))
            {
                switch (i)
                {
                    case 0: weapon.name = "Pistol"; break;
                    case 1: weapon.name = "Shotgun"; break;
                    case 2: weapon.name = "Machine Gun"; break;
                    case 3: weapon.name = "Sniper"; break;
                    default: weapon.name = $"Weapon_{i}"; break;
                }
            }
        }
        
        // Restore firepoint if it was lost
        if (firePoint == null)
        {
            CreateFirePoint();
            if (firePoint != null && firePointPosition != Vector3.zero)
            {
                firePoint.position = firePointPosition;
            }
        }
        
        // Update weapon visual
        UpdateWeaponVisual();
    }
    
    /// <summary>
    /// Help with weapon sprite setup
    /// </summary>
    [ContextMenu("Help: Setup Weapon Sprites")]
    public void HelpSetupWeaponSprites()
    {
        Debug.Log("=== WEAPON SPRITE SETUP HELP ===");
        Debug.Log("To use your original weapon sprites:");
        Debug.Log("1. Select this GameObject in the Inspector");
        Debug.Log("2. In the 'Weapon Sprites' section:");
        Debug.Log("   - Drag 'pistol.png' to 'Pistol Sprite'");
        Debug.Log("   - Drag 'shotgun.png' to 'Shotgun Sprite'");
        Debug.Log("   - Drag 'machinegun.png' to 'Machine Gun Sprite'");
        Debug.Log("   - Drag 'sniper.png' to 'Sniper Sprite'");
        Debug.Log("3. Right-click this component and select 'Initialize Weapons'");
        Debug.Log("4. The weapons should now use your original sprites!");
        Debug.Log("=== END HELP ===");
    }
    
    /// <summary>
    /// Checks if weapons have audio clips and assigns them if missing
    /// </summary>
    private IEnumerator CheckAndAssignAudioClips()
    {
        // Wait a frame to ensure AudioManager is ready
        yield return null;
        
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager.Instance is still null after waiting!");
            yield break;
        }
        
        bool needsReinitialization = false;
        
        foreach (var weapon in availableWeapons)
        {
            if (weapon.fireSound == null || weapon.reloadSound == null)
            {
                needsReinitialization = true;
                break;
            }
        }
        
        if (needsReinitialization)
        {
            ManualInitializeWeapons();
        }
    }
    
    /// <summary>
    /// Show current weapon positioning settings
    /// </summary>
    [ContextMenu("Show Current Weapon Position")]
    public void ShowCurrentWeaponPosition()
    {
        Debug.Log("=== CURRENT WEAPON POSITIONING ===");
        Debug.Log($"Weapon Scale: {weaponScale}");
        Debug.Log($"Weapon Offset X: {weaponOffsetX}");
        Debug.Log($"Weapon Offset Y: {weaponOffsetY}");
        Debug.Log($"Fire Point Offset X: {firePointOffsetX}");
        Debug.Log($"Fire Point Offset Y: {firePointOffsetY}");
        
        if (weaponHolder != null)
        {
            Debug.Log($"Current Weapon Position: {weaponHolder.localPosition}");
            Debug.Log($"Current Weapon Scale: {weaponHolder.localScale}");
        }
        
        if (firePoint != null)
        {
            Debug.Log($"Current Fire Point Position: {firePoint.localPosition}");
        }
        Debug.Log("=== END POSITIONING INFO ===");
    }
    
    /// <summary>
    /// Reset weapon positioning to default values
    /// </summary>
    [ContextMenu("Reset Weapon Position to Default")]
    public void ResetWeaponPositionToDefault()
    {
        weaponScale = 2f;
        weaponOffsetX = 0.5f;
        weaponOffsetY = 0.2f;
        firePointOffsetX = 0.8f;
        firePointOffsetY = 0.2f;
        Debug.Log("Weapon positioning reset to default values");
    }
    
    /// <summary>
    /// Set weapon positioning for smaller weapons
    /// </summary>
    [ContextMenu("Set Small Weapon Size")]
    public void SetSmallWeaponSize()
    {
        weaponScale = 1f;
        weaponOffsetX = 0.3f;
        weaponOffsetY = 0.1f;
        firePointOffsetX = 0.5f;
        firePointOffsetY = 0.1f;
        Debug.Log("Weapon positioning set for small weapons");
    }
    
    /// <summary>
    /// Set weapon positioning for larger weapons
    /// </summary>
    [ContextMenu("Set Large Weapon Size")]
    public void SetLargeWeaponSize()
    {
        weaponScale = 3f;
        weaponOffsetX = 0.8f;
        weaponOffsetY = 0.3f;
        firePointOffsetX = 1.2f;
        firePointOffsetY = 0.3f;
        Debug.Log("Weapon positioning set for large weapons");
    }
} 