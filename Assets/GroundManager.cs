using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the ground system for the Duck Game clone
/// Handles ground expansion, platform placement, and ground layer management
/// </summary>
public class GroundManager : MonoBehaviour
{
    [Header("Ground Configuration")]
    [SerializeField] private GameObject groundPrefab; // Ground tile prefab
    [SerializeField] private GameObject platformPrefab; // Platform prefab for floating platforms
    
    [Header("Ground Layout")]
    [SerializeField] private float groundWidth = 50f; // Total width of the ground
    [SerializeField] private float groundTileSize = 1f; // Size of each ground tile
    [SerializeField] private float groundY = -3f; // Y position of the ground
    
    [Header("Platform Placement")]
    [SerializeField] private int numberOfPlatforms = 5; // Number of floating platforms
    [SerializeField] private float platformMinY = 0f; // Minimum Y position for platforms
    [SerializeField] private float platformMaxY = 3f; // Maximum Y position for platforms
    [SerializeField] private float platformMinX = -20f; // Minimum X position for platforms
    [SerializeField] private float platformMaxX = 20f; // Maximum X position for platforms
    
    [Header("Ground Layer")]
    [SerializeField] private int groundLayer = 3; // Ground layer (should match TagManager)
    
    // Private variables
    private List<GameObject> groundTiles = new List<GameObject>();
    private List<GameObject> platforms = new List<GameObject>();
    
    void Start()
    {
        // Create the expanded ground
        CreateExpandedGround();
        
        // Create floating platforms
        CreateFloatingPlatforms();
    }
    
    /// <summary>
    /// Creates the expanded ground using multiple tiles
    /// </summary>
    private void CreateExpandedGround()
    {
        // Calculate how many tiles we need
        int tileCount = Mathf.CeilToInt(groundWidth / groundTileSize);
        
        for (int i = 0; i < tileCount; i++)
        {
            // Calculate position for this tile
            float xPos = (i - tileCount / 2f) * groundTileSize;
            Vector3 position = new Vector3(xPos, groundY, 0);
            
            // Create ground tile
            GameObject groundTile = CreateGroundTile(position);
            groundTiles.Add(groundTile);
        }
    }
    
    /// <summary>
    /// Creates a single ground tile at the specified position
    /// </summary>
    private GameObject CreateGroundTile(Vector3 position)
    {
        GameObject tile;
        
        if (groundPrefab != null)
        {
            // Use the prefab if available
            tile = Instantiate(groundPrefab, position, Quaternion.identity);
        }
        else
        {
            // Create a basic ground tile
            tile = CreateBasicGroundTile(position);
        }
        
        // Set the ground layer
        tile.layer = groundLayer;
        
        // Set the parent to this manager
        tile.transform.SetParent(transform);
        
        return tile;
    }
    
    /// <summary>
    /// Creates a basic ground tile when no prefab is available
    /// </summary>
    private GameObject CreateBasicGroundTile(Vector3 position)
    {
        // Create the GameObject
        GameObject tile = new GameObject("GroundTile");
        tile.transform.position = position;
        
        // Add SpriteRenderer
        SpriteRenderer spriteRenderer = tile.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateBasicSprite();
        spriteRenderer.color = new Color(0.5f, 0.3f, 0.1f, 1f); // Brown color
        
        // Add BoxCollider2D
        BoxCollider2D collider = tile.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(groundTileSize, groundTileSize);
        
        // Scale the tile to match the tile size
        tile.transform.localScale = new Vector3(groundTileSize, groundTileSize, 1);
        
        return tile;
    }
    
    /// <summary>
    /// Creates a basic sprite for ground tiles
    /// </summary>
    private Sprite CreateBasicSprite()
    {
        // Create a simple white square texture
        Texture2D texture = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];
        
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }
    
    /// <summary>
    /// Creates floating platforms for gameplay variety
    /// </summary>
    private void CreateFloatingPlatforms()
    {
        for (int i = 0; i < numberOfPlatforms; i++)
        {
            // Generate random position within bounds
            float xPos = Random.Range(platformMinX, platformMaxX);
            float yPos = Random.Range(platformMinY, platformMaxY);
            Vector3 position = new Vector3(xPos, yPos, 0);
            
            // Create platform
            GameObject platform = CreatePlatform(position);
            platforms.Add(platform);
        }
    }
    
    /// <summary>
    /// Creates a single platform at the specified position
    /// </summary>
    private GameObject CreatePlatform(Vector3 position)
    {
        GameObject platform;
        
        if (platformPrefab != null)
        {
            // Use the prefab if available
            platform = Instantiate(platformPrefab, position, Quaternion.identity);
        }
        else
        {
            // Create a basic platform
            platform = CreateBasicPlatform(position);
        }
        
        // Set the ground layer
        platform.layer = groundLayer;
        
        // Set the parent to this manager
        platform.transform.SetParent(transform);
        
        return platform;
    }
    
    /// <summary>
    /// Creates a basic platform when no prefab is available
    /// </summary>
    private GameObject CreateBasicPlatform(Vector3 position)
    {
        // Create the GameObject
        GameObject platform = new GameObject("Platform");
        platform.transform.position = position;
        
        // Add SpriteRenderer
        SpriteRenderer spriteRenderer = platform.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateBasicSprite();
        spriteRenderer.color = new Color(0.2f, 0.8f, 0.2f, 1f); // Green color
        
        // Add BoxCollider2D
        BoxCollider2D collider = platform.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(3f, 0.5f); // Wide but thin platform
        
        // Scale the platform
        platform.transform.localScale = new Vector3(3f, 0.5f, 1);
        
        return platform;
    }
    
    /// <summary>
    /// Gets the ground bounds for camera boundaries
    /// </summary>
    public Vector2 GetGroundBounds()
    {
        return new Vector2(-groundWidth / 2f, groundWidth / 2f);
    }
    
    /// <summary>
    /// Gets the ground Y position
    /// </summary>
    public float GetGroundY()
    {
        return groundY;
    }
    
    /// <summary>
    /// Adds a hazard to the ground system
    /// </summary>
    public void AddHazard(Vector3 position)
    {
        GameObject hazard = new GameObject("Hazard");
        hazard.transform.position = position;
        hazard.transform.SetParent(transform);
        
        // Add SpriteRenderer
        SpriteRenderer spriteRenderer = hazard.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateBasicSprite();
        spriteRenderer.color = Color.red;
        
        // Add BoxCollider2D as trigger
        BoxCollider2D collider = hazard.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(0.5f, 0.5f);
        
        // Add Hazard script
        hazard.AddComponent<Hazard>();
        
        // Set the hazard tag
        hazard.tag = "Hazard";
    }
    
    /// <summary>
    /// Adds a collectible to the ground system
    /// </summary>
    public void AddCollectible(Vector3 position)
    {
        GameObject collectible = new GameObject("Collectible");
        collectible.transform.position = position;
        collectible.transform.SetParent(transform);
        
        // Add SpriteRenderer
        SpriteRenderer spriteRenderer = collectible.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateBasicSprite();
        spriteRenderer.color = Color.yellow;
        
        // Add BoxCollider2D as trigger
        BoxCollider2D collider = collectible.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(0.5f, 0.5f);
        
        // Add Collectible script
        collectible.AddComponent<Collectible>();
    }
} 