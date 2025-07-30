using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    
    [Header("Jump Settings")]
    [SerializeField] private int maxJumps = 2; // Double jump = 2 jumps
    
    [Header("Health System")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float invincibilityTime = 1f; // Time player is invincible after taking damage
    
    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer = 1; // Default layer (bit 1)
    [SerializeField] private float groundCheckDistance = 0.3f; // Increased for better detection
    
    // Private variables
    private Rigidbody2D rb;
    private bool isGrounded;
    private float horizontalInput;
    private int jumpsRemaining;
    private bool wasGroundedLastFrame;
    
    // Health system variables
    private int currentHealth;
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        // Get the Rigidbody2D component attached to this GameObject
        rb = GetComponent<Rigidbody2D>();
        
        // Check if Rigidbody2D was found
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on " + gameObject.name + "! Please add a Rigidbody2D component.");
        }
        
        // Initialize jumps
        ResetJumps();
        
        // Initialize health
        currentHealth = maxHealth;
        
        // Set the player tag
        gameObject.tag = "Player";
    }

    // Update is called once per frame
    void Update()
    {
        // Get horizontal input (left/right arrow keys or A/D)
        horizontalInput = Input.GetAxis("Horizontal");
        
        // Check if we're grounded
        CheckGrounded();
        
        // Handle jumping input
        HandleJumpInput();
        
        // Update grounded state for next frame
        wasGroundedLastFrame = isGrounded;
        
        // Update invincibility timer
        UpdateInvincibility();
    }
    
    // FixedUpdate is called at a fixed time interval (good for physics)
    void FixedUpdate()
    {
        // Handle horizontal movement
        HandleMovement();
    }
    
    /// <summary>
    /// Checks if the duck is touching the ground using a raycast
    /// </summary>
    private void CheckGrounded()
    {
        // Cast a ray downward from the duck's position
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,           // Start position
            Vector2.down,                 // Direction (down)
            groundCheckDistance,          // Distance to check
            groundLayer                   // What layers to check against
        );
        
        // If we hit something, we're grounded
        isGrounded = hit.collider != null;
        
        // If we just landed (weren't grounded last frame, but are now)
        if (!wasGroundedLastFrame && isGrounded)
        {
            ResetJumps();
            
            // Play landing sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayLandSound();
            }
            
            // Play landing dust particles
            if (ParticleEffectsManager.Instance != null)
            {
                ParticleEffectsManager.Instance.PlayLandingDust(transform.position);
            }
        }
    }
    
    /// <summary>
    /// Draws debug rays in the Scene view (only visible in editor)
    /// </summary>
    private void OnDrawGizmos()
    {
        // Draw the ground check ray
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, Vector2.down * groundCheckDistance);
    }
    
    /// <summary>
    /// Handles jumping input and applies jump force
    /// </summary>
    private void HandleJumpInput()
    {
        // Check if jump button was pressed AND we have jumps remaining
        if (Input.GetButtonDown("Jump") && jumpsRemaining > 0)
        {
            // Apply upward force to make the duck jump
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            
            // Decrease jump count
            jumpsRemaining--;
            
            // Play jump sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayJumpSound();
            }
            
            // Play jump dust particles
            if (ParticleEffectsManager.Instance != null)
            {
                ParticleEffectsManager.Instance.PlayJumpDust(transform.position);
            }
        }
    }
    
    /// <summary>
    /// Handles horizontal movement using physics
    /// </summary>
    private void HandleMovement()
    {
        // Calculate the target velocity
        float targetVelocityX = horizontalInput * moveSpeed;
        
        // Set the horizontal velocity while preserving the vertical velocity (for jumping/falling)
        rb.velocity = new Vector2(targetVelocityX, rb.velocity.y);
        
        // Flip the duck and weapon based on movement direction
        if (horizontalInput > 0.01f)
        {
            if (transform.localScale.x <= 0)
            {
                transform.localScale = new Vector3(1, transform.localScale.y, 1);
            }
        }
        else if (horizontalInput < -0.01f)
        {
            if (transform.localScale.x >= 0)
            {
                transform.localScale = new Vector3(-1, transform.localScale.y, 1);
            }
        }
    }
    
    /// <summary>
    /// Resets the jump count when touching the ground
    /// </summary>
    private void ResetJumps()
    {
        jumpsRemaining = maxJumps;
    }
    
    /// <summary>
    /// Called when the duck collides with something (more reliable than raycast)
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we're colliding with the ground layer
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            // Check if we're landing from above (velocity.y <= 0 means falling)
            if (rb.velocity.y <= 0)
            {
                ResetJumps();
                
                // Play landing particles (only if we weren't grounded last frame)
                if (!wasGroundedLastFrame && ParticleEffectsManager.Instance != null)
                {
                    ParticleEffectsManager.Instance.PlayLandingDust(transform.position);
                }
            }
        }
    }
    
    /// <summary>
    /// Called when the duck enters a trigger (for hazards like spikes)
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we hit a hazard
        if (other.CompareTag("Hazard"))
        {
            TakeDamage();
        }
    }
    
    /// <summary>
    /// Takes damage and handles invincibility
    /// </summary>
    public void TakeDamage(int damage = 1)
    {
        // Don't take damage if invincible
        if (isInvincible)
        {
            return;
        }
        
        // Reduce health
        currentHealth -= damage;
        
        // Play damage sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDamageSound();
        }
        
        // Play damage particles
        if (ParticleEffectsManager.Instance != null)
        {
            ParticleEffectsManager.Instance.PlayDamageParticles(transform.position);
        }
        
        // Start invincibility
        isInvincible = true;
        invincibilityTimer = invincibilityTime;
        
        // Visual feedback (make duck flash)
        StartCoroutine(FlashEffect());
        
        // Check if dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// Updates the invincibility timer
    /// </summary>
    private void UpdateInvincibility()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
            }
        }
    }
    
    /// <summary>
    /// Makes the duck flash when taking damage
    /// </summary>
    private System.Collections.IEnumerator FlashEffect()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) yield break;
        
        Color originalColor = spriteRenderer.color;
        
        // Flash red for invincibility duration
        float flashDuration = 0.1f;
        float elapsed = 0f;
        
        while (elapsed < invincibilityTime)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
            elapsed += flashDuration * 2;
        }
        
        spriteRenderer.color = originalColor;
    }
    
    /// <summary>
    /// Handles player death
    /// </summary>
    private void Die()
    {
        // Play death sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDeathSound();
        }
        
        // Stop movement
        rb.velocity = Vector2.zero;
        
        // Disable input
        enabled = false;
        
        // You can add more death effects here (particles, sound, etc.)
    }
    
    /// <summary>
    /// Gets the current health of the duck
    /// </summary>
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    /// <summary>
    /// Gets the current grounded state
    /// </summary>
    public bool IsGrounded()
    {
        return isGrounded;
    }
}
