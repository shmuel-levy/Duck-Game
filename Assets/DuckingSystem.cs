using UnityEngine;

/// <summary>
/// Handles ducking/crouching mechanics for the Duck Game clone
/// Based on authentic Duck Game ducking mechanics
/// </summary>
public class DuckingSystem : MonoBehaviour
{
    [Header("Ducking Settings")]
    [SerializeField] private bool isDucking = false;
    [SerializeField] private float duckSpeed = 0.2f; // How fast to duck
    [SerializeField] private float duckHeight = 0.5f; // How much to shrink when ducking
    [SerializeField] private float normalHeight = 1f; // Normal height
    
    [Header("Ducking Input")]
    [SerializeField] private KeyCode duckKey = KeyCode.S; // S key for ducking
    [SerializeField] private bool canDuck = true;
    
    [Header("Ducking Effects")]
    [SerializeField] private AudioClip duckSound;
    [SerializeField] private AudioClip standSound;
    [SerializeField] private GameObject duckDustEffect;
    
    // Private variables
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Vector3 originalScale;
    private Vector2 originalColliderSize;
    private bool wasDuckingLastFrame = false;
    
    // Events
    public System.Action<bool> OnDuckingChanged; // true = started ducking, false = stopped ducking
    
    void Start()
    {
        // Get components
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        
        // Store original values
        if (transform != null)
        {
            originalScale = transform.localScale;
        }
        
        if (boxCollider != null)
        {
            originalColliderSize = boxCollider.size;
        }
    }
    
    void Update()
    {
        // Only allow ducking when grounded
        DuckController duckController = GetComponent<DuckController>();
        bool isGrounded = duckController != null ? duckController.IsGrounded() : true;
        if (!isGrounded && isDucking)
        {
            SetDucking(false); // Stand up if not grounded
        }
        else if (isGrounded)
        {
            HandleDuckingInput();
        }
        UpdateDuckingAnimation();
    }
    
    /// <summary>
    /// Handles ducking input
    /// </summary>
    private void HandleDuckingInput()
    {
        if (!canDuck) return;
        
        // Check for ducking input
bool shouldDuck = Input.GetKey(duckKey);        
        // Update ducking state instantly
        if (shouldDuck != isDucking)
        {
            SetDucking(shouldDuck);
        }
    }
    
    /// <summary>
    /// Updates the ducking animation
    /// </summary>
    private void UpdateDuckingAnimation()
    {
        if (isDucking != wasDuckingLastFrame)
        {
            // Play ducking effects
            if (isDucking)
            {
                PlayDuckEffects();
            }
            else
            {
                PlayStandEffects();
            }
            
            wasDuckingLastFrame = isDucking;
        }
    }
    
    /// <summary>
    /// Sets the ducking state
    /// </summary>
    public void SetDucking(bool ducking)
    {
        if (isDucking == ducking) return;
        
        isDucking = ducking;
        
        // Animate the ducking
        StartCoroutine(AnimateDucking(ducking));
        
        // Trigger event
        OnDuckingChanged?.Invoke(ducking);
    }
    
    /// <summary>
    /// Animates the ducking transition
    /// </summary>
    private System.Collections.IEnumerator AnimateDucking(bool ducking)
    {
        float targetHeight = ducking ? duckHeight : normalHeight;
        float currentHeight = transform.localScale.y;
        float startHeight = currentHeight;
        float elapsed = 0f;
        
        while (elapsed < duckSpeed)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duckSpeed;
            
            // Smooth interpolation
            float newHeight = Mathf.Lerp(startHeight, targetHeight, progress);
            
            // Update scale
            Vector3 newScale = transform.localScale;
            newScale.y = newHeight;
            transform.localScale = newScale;
            
            // Update collider size
            if (boxCollider != null)
            {
                Vector2 newColliderSize = originalColliderSize;
                newColliderSize.y *= newHeight / normalHeight;
                boxCollider.size = newColliderSize;
            }
            
            yield return null;
        }
        
        // Ensure final values are exact
        Vector3 finalScale = transform.localScale;
        finalScale.y = targetHeight;
        transform.localScale = finalScale;
        
        if (boxCollider != null)
        {
            Vector2 finalColliderSize = originalColliderSize;
            finalColliderSize.y *= targetHeight / normalHeight;
            boxCollider.size = finalColliderSize;
        }
    }
    
    /// <summary>
    /// Plays effects when starting to duck
    /// </summary>
    private void PlayDuckEffects()
    {
        // Play duck sound
        if (AudioManager.Instance != null && duckSound != null)
        {
            AudioManager.Instance.PlaySound(duckSound);
        }
        
        // Play duck dust particles
        if (ParticleEffectsManager.Instance != null)
        {
            ParticleEffectsManager.Instance.PlayJumpDust(transform.position);
        }
        
        // Create duck dust effect
        if (duckDustEffect != null)
        {
            Instantiate(duckDustEffect, transform.position, Quaternion.identity);
        }
    }
    
    /// <summary>
    /// Plays effects when standing up
    /// </summary>
    private void PlayStandEffects()
    {
        // Play stand sound
        if (AudioManager.Instance != null && standSound != null)
        {
            AudioManager.Instance.PlaySound(standSound);
        }
        
        // Play stand dust particles
        if (ParticleEffectsManager.Instance != null)
        {
            ParticleEffectsManager.Instance.PlayJumpDust(transform.position);
        }
    }
    
    /// <summary>
    /// Gets the current ducking state
    /// </summary>
    public bool IsDucking()
    {
        return isDucking;
    }
    
    /// <summary>
    /// Forces the duck to stand up
    /// </summary>
    public void ForceStand()
    {
        if (isDucking)
        {
            SetDucking(false);
        }
    }
    
    /// <summary>
    /// Enables or disables ducking
    /// </summary>
    public void SetCanDuck(bool canDuckNow)
    {
        canDuck = canDuckNow;
        
        // If ducking is disabled, force stand
        if (!canDuck && isDucking)
        {
            ForceStand();
        }
    }
    
    /// <summary>
    /// Gets the duck height multiplier (1 = normal, 0.5 = ducked)
    /// </summary>
    public float GetHeightMultiplier()
    {
        return isDucking ? duckHeight / normalHeight : 1f;
    }
    
    /// <summary>
    /// Checks if the duck can fit through a gap
    /// </summary>
    public bool CanFitThroughGap(float gapHeight)
    {
        float duckHeight = transform.localScale.y;
        return duckHeight <= gapHeight;
    }
} 