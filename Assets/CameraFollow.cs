using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Advanced camera follow system for 2D platformer games
/// Based on Unity's best practices and industry standards
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target; // The duck to follow
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -10); // Camera offset from target
    
    [Header("Follow Settings")]
    [SerializeField] private float lookAheadDistance = 3f; // How far ahead to look based on movement
    [SerializeField] private float lookAheadSpeed = 2f; // How fast look-ahead responds to movement
    
    [Header("Boundary Settings")]
    [SerializeField] private bool useBoundaries = true; // Whether to constrain camera movement
    [SerializeField] private float minX = -25f; // Left boundary (expanded for new ground)
    [SerializeField] private float maxX = 25f;  // Right boundary (expanded for new ground)
    [SerializeField] private float minY = -5f;  // Bottom boundary
    [SerializeField] private float maxY = 10f;  // Top boundary (increased for platforms)
    
    [Header("Smooth Damping")]
    [SerializeField] private float smoothTime = 0.3f; // Smoothing time for camera movement
    [SerializeField] private float maxSpeed = 10f; // Maximum camera speed
    
    // Private variables
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 lookAheadPos = Vector3.zero;
    private Vector3 targetPosition;
    private Camera cam;
    
    void Start()
    {
        // Get the camera component
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("CameraFollow: No Camera component found!");
            return;
        }
        
        // Find the duck if target is not set
        if (target == null)
        {
            GameObject duck = GameObject.FindGameObjectWithTag("Player");
            if (duck != null)
            {
                target = duck.transform;
            }
            else
            {
                Debug.LogWarning("CameraFollow: No target set and no Player found! Please assign a target.");
            }
        }
        
        // Set initial position
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        // Calculate look-ahead position based on target's velocity
        CalculateLookAhead();
        
        // Calculate target position
        CalculateTargetPosition();
        
        // Apply boundaries if enabled
        if (useBoundaries)
        {
            ApplyBoundaries();
        }
        
        // Smoothly move camera to target position
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            targetPosition, 
            ref currentVelocity, 
            smoothTime, 
            maxSpeed
        );
    }
    
    /// <summary>
    /// Calculates the look-ahead position based on target's movement
    /// </summary>
    private void CalculateLookAhead()
    {
        // Get target's velocity (if it has a Rigidbody2D)
        Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
        Vector3 targetVelocity = Vector3.zero;
        
        if (targetRb != null)
        {
            targetVelocity = targetRb.velocity;
        }
        
        // Calculate look-ahead based on horizontal movement
        float lookAheadX = Mathf.Sign(targetVelocity.x) * lookAheadDistance;
        
        // Smoothly interpolate look-ahead position
        Vector3 desiredLookAhead = new Vector3(lookAheadX, 0, 0);
        lookAheadPos = Vector3.Lerp(lookAheadPos, desiredLookAhead, lookAheadSpeed * Time.deltaTime);
    }
    
    /// <summary>
    /// Calculates the final target position for the camera
    /// </summary>
    private void CalculateTargetPosition()
    {
        // Base position: target position + offset + look-ahead
        targetPosition = target.position + offset + lookAheadPos;
        
        // Keep the camera's Z position unchanged
        targetPosition.z = transform.position.z;
    }
    
    /// <summary>
    /// Applies boundary constraints to the camera position
    /// </summary>
    private void ApplyBoundaries()
    {
        // Calculate camera viewport bounds
        float cameraHeight = cam.orthographicSize;
        float cameraWidth = cameraHeight * cam.aspect;
        
        // Clamp X position
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX + cameraWidth, maxX - cameraWidth);
        
        // Clamp Y position
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY + cameraHeight, maxY - cameraHeight);
    }
    
    /// <summary>
    /// Sets the target for the camera to follow
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    /// <summary>
    /// Enables or disables boundary constraints
    /// </summary>
    public void SetBoundaries(bool enabled, float minX, float maxX, float minY, float maxY)
    {
        useBoundaries = enabled;
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
    }
    
    /// <summary>
    /// Draws boundary gizmos in the Scene view for easy setup
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (!useBoundaries) return;
        
        Gizmos.color = Color.yellow;
        
        // Draw boundary rectangle
        Vector3 topLeft = new Vector3(minX, maxY, 0);
        Vector3 topRight = new Vector3(maxX, maxY, 0);
        Vector3 bottomLeft = new Vector3(minX, minY, 0);
        Vector3 bottomRight = new Vector3(maxX, minY, 0);
        
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
} 