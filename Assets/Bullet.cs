using UnityEngine;

/// <summary>
/// Handles bullet physics, damage, and lifetime
/// Based on Duck Game bullet mechanics
/// </summary>
public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private bool destroyOnHit = true;
    [SerializeField] private LayerMask hitLayers = -1; // All layers by default
    
    [Header("Effects")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private AudioClip hitSound;
    
    private float spawnTime;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        spawnTime = Time.time;
        
        // Get sprite renderer for visibility
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Make bullet more visible
            spriteRenderer.color = Color.yellow;
            spriteRenderer.sortingOrder = 10;
        }
        
        // Destroy bullet after lifetime
        Destroy(gameObject, lifetime);
    }
    
    void Update()
    {
        // Check if bullet is out of bounds
        if (transform.position.magnitude > 100f)
        {
            Destroy(gameObject);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Don't hit the player who fired the bullet
        if (other.CompareTag("Player"))
        {
            return;
        }
        
        // Check if we hit something we should damage
        if (((1 << other.gameObject.layer) & hitLayers) != 0)
        {
            HandleHit(other);
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Don't hit the player who fired the bullet
        if (collision.gameObject.CompareTag("Player"))
        {
            return;
        }
        
        // Check if we hit something we should damage
        if (((1 << collision.gameObject.layer) & hitLayers) != 0)
        {
            HandleHit(collision.collider);
        }
    }
    
    /// <summary>
    /// Handles what happens when the bullet hits something
    /// </summary>
    private void HandleHit(Collider2D hitObject)
    {
        // Deal damage to enemy if it's an enemy
        if (hitObject.CompareTag("Enemy"))
        {
            // You can add enemy damage logic here
        }
        
        // Play hit effects
        PlayHitEffects();
        
        // Destroy bullet if it should be destroyed on hit
        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Plays hit effects (particles and sound)
    /// </summary>
    private void PlayHitEffects()
    {
        // Play hit sound
        if (AudioManager.Instance != null && hitSound != null)
        {
            AudioManager.Instance.PlaySound(hitSound);
        }
        
        // Create hit effect
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            // Create a simple hit effect
            CreateSimpleHitEffect();
        }
    }
    
    /// <summary>
    /// Creates a simple hit effect
    /// </summary>
    private void CreateSimpleHitEffect()
    {
        // Create a small explosion effect
        GameObject hitEffect = new GameObject("HitEffect");
        hitEffect.transform.position = transform.position;
        
        // Add particle system
        ParticleSystem ps = hitEffect.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 0.5f;
        main.startSpeed = 2f;
        main.startSize = 0.1f;
        main.maxParticles = 10;
        main.startColor = Color.yellow;
        
        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0.0f, 10)
        });
        
        // Destroy the effect after it's done
        Destroy(hitEffect, 1f);
    }
    
    /// <summary>
    /// Initializes the bullet with custom damage and lifetime
    /// </summary>
    public void Initialize(float bulletDamage, float bulletLifetime)
    {
        damage = bulletDamage;
        lifetime = bulletLifetime;
    }
    
    /// <summary>
    /// Gets the damage of this bullet
    /// </summary>
    public float GetDamage()
    {
        return damage;
    }
} 