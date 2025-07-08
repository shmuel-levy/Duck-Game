using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collectible items that players can gather for points
/// Based on Unity best practices for collectibles
/// </summary>
public class Collectible : MonoBehaviour
{
    [Header("Collectible Settings")]
    [SerializeField] private int pointValue = 10; // Points awarded when collected
    [SerializeField] private bool isCollected = false; // Track if already collected
    
    [Header("Visual Effects")]
    [SerializeField] private Color collectibleColor = Color.yellow; // Color of the collectible
    [SerializeField] private float rotationSpeed = 90f; // Degrees per second rotation
    [SerializeField] private float bobSpeed = 2f; // Speed of up/down movement
    [SerializeField] private float bobHeight = 0.2f; // Height of up/down movement
    
    [Header("Collection Effects")]
    [SerializeField] private float collectionScale = 1.5f; // Scale when collected
    [SerializeField] private float collectionDuration = 0.3f; // How long collection effect lasts
    
    // Private variables
    private SpriteRenderer spriteRenderer;
    private Vector3 startPosition;
    private float bobTimer = 0f;
    private bool isCollecting = false;
    
    void Start()
    {
        // Get the sprite renderer and set the collectible color
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = collectibleColor;
        }
        
        // Store the starting position for bobbing animation
        startPosition = transform.position;
    }
    
    void Update()
    {
        if (!isCollected && !isCollecting)
        {
            // Rotate the collectible
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            
            // Bob up and down
            BobAnimation();
        }
    }
    
    /// <summary>
    /// Makes the collectible bob up and down for visual appeal
    /// </summary>
    private void BobAnimation()
    {
        bobTimer += Time.deltaTime * bobSpeed;
        float bobOffset = Mathf.Sin(bobTimer) * bobHeight;
        transform.position = startPosition + Vector3.up * bobOffset;
    }
    
    /// <summary>
    /// Called when the player touches this collectible
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player entered the collectible
        if (other.CompareTag("Player") && !isCollected && !isCollecting)
        {
            Collect();
        }
    }
    
    /// <summary>
    /// Handles the collection of this item
    /// </summary>
    private void Collect()
    {
        if (isCollected || isCollecting) return;
        
        isCollecting = true;
        isCollected = true;
        
        // Add points to the game manager
        GameManager.Instance?.AddScore(pointValue);
        
        // Play coin collection sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCoinSound();
        }
        
        // Play coin sparkle particles
        if (ParticleEffectsManager.Instance != null)
        {
            ParticleEffectsManager.Instance.PlayCoinSparkle(transform.position);
        }
        
        // Play collection effect
        StartCoroutine(CollectionEffect());
    }
    
    /// <summary>
    /// Visual effect when the collectible is collected
    /// </summary>
    private IEnumerator CollectionEffect()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * collectionScale;
        
        // Scale up
        float elapsed = 0f;
        while (elapsed < collectionDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / (collectionDuration * 0.5f);
            transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            yield return null;
        }
        
        // Scale down and fade out
        elapsed = 0f;
        while (elapsed < collectionDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / (collectionDuration * 0.5f);
            transform.localScale = Vector3.Lerp(targetScale, Vector3.zero, progress);
            
            // Fade out the sprite
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 1f - progress;
                spriteRenderer.color = color;
            }
            
            yield return null;
        }
        
        // Destroy the collectible
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Draws a gizmo in the Scene view to show the collectible area
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = collectibleColor;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
} 