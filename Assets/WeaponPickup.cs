using UnityEngine;

/// <summary>
/// Handles weapon pickups in the game world
/// </summary>
public class WeaponPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private WeaponSystem.WeaponType weaponType;
    [SerializeField] private int ammoAmount = 30;
    [SerializeField] private bool isAmmoPickup = false;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.2f;
    
    [Header("Visual Effects")]
    [SerializeField] private Color weaponColor = Color.blue;
    [SerializeField] private Color ammoColor = Color.green;
    [SerializeField] private GameObject pickupEffect;
    
    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Set color based on pickup type
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isAmmoPickup ? ammoColor : weaponColor;
        }
    }
    
    void Update()
    {
        // Rotate the pickup
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        
        // Bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            WeaponSystem weaponSystem = other.GetComponent<WeaponSystem>();
            if (weaponSystem != null)
            {
                if (isAmmoPickup)
                {
                    // Add ammo to current weapon
                    weaponSystem.AddAmmo(ammoAmount);
                }
                else
                {
                    // Unlock weapon
                    weaponSystem.UnlockWeapon(weaponType);
                }
                
                // Play pickup effect
                PlayPickupEffect();
                
                // Destroy the pickup
                Destroy(gameObject);
            }
        }
    }
    
    /// <summary>
    /// Plays pickup effects
    /// </summary>
    private void PlayPickupEffect()
    {
        // Play pickup sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound("coin"); // Use coin sound for pickup
        }
        
        // Create pickup effect
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }
        else
        {
            // Create simple sparkle effect
            CreateSimplePickupEffect();
        }
    }
    
    /// <summary>
    /// Creates a simple pickup effect
    /// </summary>
    private void CreateSimplePickupEffect()
    {
        GameObject effect = new GameObject("PickupEffect");
        effect.transform.position = transform.position;
        
        // Add particle system
        ParticleSystem ps = effect.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 1f;
        main.startSpeed = 3f;
        main.startSize = 0.1f;
        main.maxParticles = 20;
        main.startColor = isAmmoPickup ? ammoColor : weaponColor;
        
        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0.0f, 20)
        });
        
        // Destroy the effect after it's done
        Destroy(effect, 2f);
    }
} 