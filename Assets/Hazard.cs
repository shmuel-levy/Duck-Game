using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [Header("Hazard Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private bool isLethal = false; // If true, kills player instantly
    
    // Public property to access damage value
    public int Damage => damage;
    
    [Header("Visual Effects")]
    [SerializeField] private Color hazardColor = Color.red;
    
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        // Get the sprite renderer and set the hazard color
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hazardColor;
        }
        
        // Make sure this object has the "Hazard" tag
        if (gameObject.tag != "Hazard")
        {
            Debug.LogWarning($"Hazard {gameObject.name} should have 'Hazard' tag!");
        }
    }
    
    /// <summary>
    /// Called when a player enters this hazard's trigger
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player entered the hazard
        if (other.CompareTag("Player"))
        {
            DuckController player = other.GetComponent<DuckController>();
            if (player != null)
            {
                if (isLethal)
                {
                    // Kill the player instantly
                    player.TakeDamage();
                    player.TakeDamage(); // Take damage multiple times to kill
                    player.TakeDamage();
                }
                else
                {
                    // Deal normal damage
                    player.TakeDamage();
                }
            }
        }
    }
} 