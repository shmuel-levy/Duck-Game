using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Automatically sets up the game environment with expanded ground, collectibles, and hazards
/// This script should be attached to an empty GameObject in the scene
/// </summary>
public class GameSetup : MonoBehaviour
{
    [Header("Setup Configuration")]
    [SerializeField] private bool autoSetup = true; // Whether to run setup automatically
    [SerializeField] private int numberOfCollectibles = 10; // Number of collectibles to spawn
    [SerializeField] private int numberOfHazards = 5; // Number of hazards to spawn
    
    [Header("Spawn Bounds")]
    [SerializeField] private float spawnMinX = -20f; // Minimum X for spawning
    [SerializeField] private float spawnMaxX = 20f;  // Maximum X for spawning
    [SerializeField] private float spawnMinY = -2f;  // Minimum Y for spawning
    [SerializeField] private float spawnMaxY = 4f;   // Maximum Y for spawning
    
    private GroundManager groundManager;
    
    void Start()
    {
        if (autoSetup)
        {
            SetupGame();
        }
    }
    
    /// <summary>
    /// Sets up the complete game environment
    /// </summary>
    public void SetupGame()
    {
        // Create GroundManager if it doesn't exist
        CreateGroundManager();
        
        // Wait a frame for GroundManager to initialize
        StartCoroutine(SetupAfterGroundManager());
    }
    
    /// <summary>
    /// Creates the GroundManager GameObject and component
    /// </summary>
    private void CreateGroundManager()
    {
        // Check if GroundManager already exists
        groundManager = FindObjectOfType<GroundManager>();
        
        if (groundManager == null)
        {
            // Create GroundManager GameObject
            GameObject groundManagerObj = new GameObject("GroundManager");
            groundManager = groundManagerObj.AddComponent<GroundManager>();
        }
    }
    
    /// <summary>
    /// Sets up collectibles and hazards after GroundManager is ready
    /// </summary>
    private IEnumerator SetupAfterGroundManager()
    {
        // Wait for GroundManager to initialize
        yield return new WaitForEndOfFrame();
        
        // Spawn collectibles
        SpawnCollectibles();
        
        // Spawn hazards
        SpawnHazards();
        
        // Update existing ground objects to use the correct layer
        UpdateExistingGroundObjects();
    }
    
    /// <summary>
    /// Spawns collectibles at random positions
    /// </summary>
    private void SpawnCollectibles()
    {
        for (int i = 0; i < numberOfCollectibles; i++)
        {
            Vector3 position = GetRandomSpawnPosition();
            
            // Create collectible
            GameObject collectible = new GameObject($"Collectible_{i}");
            collectible.transform.position = position;
            
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
    
    /// <summary>
    /// Spawns hazards at random positions
    /// </summary>
    private void SpawnHazards()
    {
        for (int i = 0; i < numberOfHazards; i++)
        {
            Vector3 position = GetRandomSpawnPosition();
            
            // Create hazard
            GameObject hazard = new GameObject($"Hazard_{i}");
            hazard.transform.position = position;
            
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
    }
    
    /// <summary>
    /// Updates existing ground objects to use the correct layer
    /// </summary>
    private void UpdateExistingGroundObjects()
    {
        // Find all objects with "Ground" or "Platform" in their name
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Ground") || obj.name.Contains("Platform"))
            {
                // Set to Ground layer (layer 3)
                obj.layer = 3;
            }
        }
    }
    
    /// <summary>
    /// Gets a random spawn position within the defined bounds
    /// </summary>
    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(spawnMinX, spawnMaxX);
        float y = Random.Range(spawnMinY, spawnMaxY);
        return new Vector3(x, y, 0);
    }
    
    /// <summary>
    /// Creates a basic sprite for objects
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
} 