using UnityEngine;

/// <summary>
/// Simple test script to debug ground detection and effects
/// Attach this to your Duck GameObject
/// </summary>
public class GroundTest : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private LayerMask groundLayer = 1; // Default layer
    [SerializeField] private float groundCheckDistance = 0.5f;
    
    private bool isGrounded = false;
    private bool wasGroundedLastFrame = false;
    
    void Update()
    {
        // Test ground detection
        CheckGround();
        
        // Test effects when landing
        if (!wasGroundedLastFrame && isGrounded)
        {
            TestLandingEffects();
        }
        
        wasGroundedLastFrame = isGrounded;
    }
    
    void CheckGround()
    {
        // Cast a ray downward
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
        
        isGrounded = hit.collider != null;
    }
    
    void TestLandingEffects()
    {
        // Test audio
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLandSound();
        }
        
        // Test particles
        if (ParticleEffectsManager.Instance != null)
        {
            ParticleEffectsManager.Instance.PlayLandingDust(transform.position);
        }
    }
    
    void OnDrawGizmos()
    {
        // Draw the ground check ray
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, Vector2.down * groundCheckDistance);
    }
} 