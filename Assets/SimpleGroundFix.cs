using UnityEngine;

/// <summary>
/// Simple script to fix ground detection and expand the game area
/// Attach this to an empty GameObject in the scene
/// </summary>
public class SimpleGroundFix : MonoBehaviour
{
    [Header("Ground Fix")]
    [SerializeField] private bool fixGroundLayers = true;
    [SerializeField] private bool expandGround = true;
    [SerializeField] private bool addCollectibles = true;
    [SerializeField] private bool addHazards = true;
    
    void Start()
    {
        if (fixGroundLayers)
            FixGroundLayers();
            
        if (expandGround)
            ExpandGround();
            
        if (addCollectibles)
            AddCollectibles();
            
        if (addHazards)
            AddHazards();
    }
    
    /// <summary>
    /// Fixes the layer of existing ground objects
    /// </summary>
    private void FixGroundLayers()
    {
        // Find and fix ground objects
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Ground") || obj.name.Contains("Platform"))
            {
                obj.layer = 3; // Ground layer
            }
        }
    }
    
    /// <summary>
    /// Expands the existing ground
    /// </summary>
    private void ExpandGround()
    {
        GameObject ground = GameObject.Find("Ground");
        if (ground != null)
        {
            ground.transform.localScale = new Vector3(50, 1, 1);
        }
    }
    
    /// <summary>
    /// Adds some collectibles to the scene
    /// </summary>
    private void AddCollectibles()
    {
        for (int i = 0; i < 8; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(-20f, 20f),
                Random.Range(-2f, 3f),
                0
            );
            
            CreateCollectible(position, $"Collectible_{i}");
        }
    }
    
    /// <summary>
    /// Adds some hazards to the scene
    /// </summary>
    private void AddHazards()
    {
        for (int i = 0; i < 4; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(-20f, 20f),
                Random.Range(-2f, 2f),
                0
            );
            
            CreateHazard(position, $"Hazard_{i}");
        }
    }
    
    /// <summary>
    /// Creates a collectible at the specified position
    /// </summary>
    private void CreateCollectible(Vector3 position, string name)
    {
        GameObject collectible = new GameObject(name);
        collectible.transform.position = position;
        
        // Add SpriteRenderer
        SpriteRenderer spriteRenderer = collectible.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateBasicSprite();
        spriteRenderer.color = Color.yellow;
        
        // Add BoxCollider2D
        BoxCollider2D collider = collectible.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(0.5f, 0.5f);
        
        // Add Collectible script
        collectible.AddComponent<Collectible>();
    }
    
    /// <summary>
    /// Creates a hazard at the specified position
    /// </summary>
    private void CreateHazard(Vector3 position, string name)
    {
        GameObject hazard = new GameObject(name);
        hazard.transform.position = position;
        
        // Add SpriteRenderer
        SpriteRenderer spriteRenderer = hazard.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateBasicSprite();
        spriteRenderer.color = Color.red;
        
        // Add BoxCollider2D
        BoxCollider2D collider = hazard.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(0.5f, 0.5f);
        
        // Add Hazard script
        hazard.AddComponent<Hazard>();
        
        // Set tag
        hazard.tag = "Hazard";
    }
    
    /// <summary>
    /// Creates a basic sprite
    /// </summary>
    private Sprite CreateBasicSprite()
    {
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