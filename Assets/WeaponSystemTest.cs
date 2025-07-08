using UnityEngine;

/// <summary>
/// Simple test script to verify script execution and help debug WeaponSystem issues
/// </summary>
public class WeaponSystemTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== WEAPON SYSTEM TEST START ===");
        
        // Check if WeaponSystem component exists on this GameObject
        WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
        if (weaponSystem != null)
        {
            Debug.Log("✓ WeaponSystem component found on this GameObject");
        }
        else
        {
            Debug.LogError("✗ WeaponSystem component NOT found on this GameObject!");
            Debug.Log("Please add the WeaponSystem component to this GameObject.");
        }
        
        // Check if this GameObject has a name
        Debug.Log($"GameObject name: {gameObject.name}");
        
        // Check if this GameObject is active
        Debug.Log($"GameObject active: {gameObject.activeInHierarchy}");
        
        // Check if this script is enabled
        Debug.Log($"This script enabled: {enabled}");
    }
    
    void Update()
    {
        // Test input detection
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("=== SPACE KEY PRESSED ===");
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("=== LEFT MOUSE BUTTON PRESSED ===");
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("=== T KEY PRESSED (TEST KEY) ===");
            TestWeaponSystem();
        }
    }
    
    void TestWeaponSystem()
    {
        Debug.Log("=== TESTING WEAPON SYSTEM ===");
        
        WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
        if (weaponSystem != null)
        {
            Debug.Log("Calling TestShoot()...");
            weaponSystem.TestShoot();
        }
        else
        {
            Debug.LogError("Cannot test WeaponSystem - component not found!");
        }
    }
} 