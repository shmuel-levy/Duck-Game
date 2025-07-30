using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] private bool isMoving = false;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Vector2 moveDirection = Vector2.right;
    [SerializeField] private float moveDistance = 3f;
    
    [Header("Visual Settings")]
    [SerializeField] private Color platformColor = Color.green;
    
    private SpriteRenderer spriteRenderer;
    private Vector3 startPosition;
    private float moveTimer = 0f;
    
    void Start()
    {
        // Get the sprite renderer and set the platform color
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = platformColor;
        }
        
        // Store the starting position for moving platforms
        startPosition = transform.position;
    }
    
    void Update()
    {
        // Handle moving platform logic
        if (isMoving)
        {
            MovePlatform();
        }
    }
    
    /// <summary>
    /// Makes the platform move back and forth
    /// </summary>
    private void MovePlatform()
    {
        moveTimer += Time.deltaTime * moveSpeed;
        
        // Use sine wave for smooth back-and-forth movement
        float offset = Mathf.Sin(moveTimer) * moveDistance;
        Vector3 newPosition = startPosition + (Vector3)(moveDirection.normalized * offset);
        
        transform.position = newPosition;
    }
    
    /// <summary>
    /// Called when the player lands on this platform
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Make the player a child of the platform so they move with it
            if (isMoving)
            {
                collision.transform.SetParent(transform);
            }
        }
    }
    
    /// <summary>
    /// Called when the player leaves this platform
    /// </summary>
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Remove the player from being a child of the platform
            collision.transform.SetParent(null);
        }
    }
} 