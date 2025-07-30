using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DuckGame.Core;
using DuckGame.Managers;

namespace DuckGame.Controllers
{
    /// <summary>
    /// Player controller with event-driven architecture
    /// </summary>
    public class DuckController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 10f;
        
        [Header("Jump Settings")]
        [SerializeField] private int maxJumps = 2;
        
        [Header("Health System")]
        [SerializeField] private int maxHealth = 3;
        [SerializeField] private float invincibilityTime = 1f;
        
        [Header("Ground Detection")]
        [SerializeField] private LayerMask groundLayer = 1;
        [SerializeField] private float groundCheckDistance = 0.3f;
        
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
        
        // Player data
        private PlayerData playerData;
        
        void Start()
        {
            // Get the Rigidbody2D component
            rb = GetComponent<Rigidbody2D>();
            
            if (rb == null)
            {
                Debug.LogError("Rigidbody2D not found on " + gameObject.name + "! Please add a Rigidbody2D component.");
            }
            
            // Initialize player data
            InitializePlayerData();
            
            // Set the player tag
            gameObject.tag = "Player";
        }
        
        void Update()
        {
            // Get horizontal input
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
        
        void FixedUpdate()
        {
            // Handle horizontal movement
            HandleMovement();
        }
        
        /// <summary>
        /// Initializes player data
        /// </summary>
        private void InitializePlayerData()
        {
            playerData = new PlayerData();
            currentHealth = maxHealth;
            ResetJumps();
            
            // Notify GameManager of initial health
            GameEvents.OnPlayerHealthChanged.Invoke(currentHealth);
        }
        
        /// <summary>
        /// Checks if the duck is touching the ground using a raycast
        /// </summary>
        private void CheckGrounded()
        {
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                Vector2.down,
                groundCheckDistance,
                groundLayer
            );
            
            isGrounded = hit.collider != null;
            playerData.isGrounded = isGrounded;
            
            // If we just landed
            if (!wasGroundedLastFrame && isGrounded)
            {
                ResetJumps();
                GameEvents.OnPlayerLand.Invoke();
                
                // Play landing particles
                GameEvents.OnPlayParticle.Invoke(transform.position, "landingDust");
            }
        }
        
        /// <summary>
        /// Draws debug rays in the Scene view
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, Vector2.down * groundCheckDistance);
        }
        
        /// <summary>
        /// Handles jumping input and applies jump force
        /// </summary>
        private void HandleJumpInput()
        {
            if (Input.GetButtonDown("Jump") && jumpsRemaining > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpsRemaining--;
                playerData.jumpsRemaining = jumpsRemaining;
                
                GameEvents.OnPlayerJump.Invoke();
                GameEvents.OnPlayParticle.Invoke(transform.position, "jumpDust");
            }
        }
        
        /// <summary>
        /// Handles horizontal movement using physics
        /// </summary>
        private void HandleMovement()
        {
            float targetVelocityX = horizontalInput * moveSpeed;
            rb.velocity = new Vector2(targetVelocityX, rb.velocity.y);
            
            // Flip the duck based on movement direction
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
            
            // Update player data
            playerData.position = transform.position;
        }
        
        /// <summary>
        /// Resets the jump count when touching the ground
        /// </summary>
        private void ResetJumps()
        {
            jumpsRemaining = maxJumps;
            playerData.jumpsRemaining = jumpsRemaining;
        }
        
        /// <summary>
        /// Called when the duck collides with something
        /// </summary>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (((1 << collision.gameObject.layer) & groundLayer) != 0)
            {
                if (rb.velocity.y <= 0)
                {
                    ResetJumps();
                    
                    if (!wasGroundedLastFrame)
                    {
                        GameEvents.OnPlayParticle.Invoke(transform.position, "landingDust");
                    }
                }
            }
        }
        
        /// <summary>
        /// Called when the duck enters a trigger
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
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
            if (isInvincible) return;
            
            currentHealth -= damage;
            playerData.health = currentHealth;
            
            GameEvents.OnPlayerTakeDamage.Invoke();
            GameEvents.OnPlayerHealthChanged.Invoke(currentHealth);
            GameEvents.OnPlayParticle.Invoke(transform.position, "damageParticles");
            
            // Start invincibility
            isInvincible = true;
            invincibilityTimer = invincibilityTime;
            playerData.isInvincible = true;
            
            // Visual feedback
            StartCoroutine(FlashEffect());
            
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
                    playerData.isInvincible = false;
                }
            }
        }
        
        /// <summary>
        /// Makes the duck flash when taking damage
        /// </summary>
        private IEnumerator FlashEffect()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) yield break;
            
            Color originalColor = spriteRenderer.color;
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
            GameEvents.OnPlayerDeath.Invoke();
            
            // Stop movement
            rb.velocity = Vector2.zero;
            
            // Disable input
            enabled = false;
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
        
        /// <summary>
        /// Gets the player data
        /// </summary>
        public PlayerData GetPlayerData()
        {
            return playerData;
        }
        
        /// <summary>
        /// Heals the player
        /// </summary>
        public void Heal(int amount)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            playerData.health = currentHealth;
            GameEvents.OnPlayerHealthChanged.Invoke(currentHealth);
        }
    }
}
