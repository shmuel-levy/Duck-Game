using UnityEngine;

public class SimpleWeaponSwitcher : MonoBehaviour
{
    public Sprite[] weaponSprites; // Assign your weapon sprites in the Inspector
    private int currentWeaponIndex = 0;
    private SpriteRenderer weaponRenderer;
    private Transform weaponSprite;

    void Start()
    {
        weaponSprite = transform.Find("WeaponHolder/WeaponSprite");
        if (weaponSprite == null)
        {
            Debug.LogError("WeaponSprite not found! Check your hierarchy: Duck > WeaponHolder > WeaponSprite");
            return;
        }
        weaponRenderer = weaponSprite.GetComponent<SpriteRenderer>();
        if (weaponRenderer == null)
        {
            Debug.LogError("WeaponSprite has no SpriteRenderer! Add one and assign a sprite.");
            return;
        }
        UpdateWeaponSprite();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            NextWeapon();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            PreviousWeapon();
        }
    }

    void NextWeapon()
    {
        if (weaponSprites.Length == 0) return;
        currentWeaponIndex = (currentWeaponIndex + 1) % weaponSprites.Length;
        UpdateWeaponSprite();
        PlayWeaponSwitchSound();
    }

    void PreviousWeapon()
    {
        if (weaponSprites.Length == 0) return;
        currentWeaponIndex = (currentWeaponIndex - 1 + weaponSprites.Length) % weaponSprites.Length;
        UpdateWeaponSprite();
        PlayWeaponSwitchSound();
    }

    void UpdateWeaponSprite()
    {
        if (weaponRenderer != null && weaponSprites.Length > 0)
        {
            weaponRenderer.sprite = weaponSprites[currentWeaponIndex];
        }
    }
    
    // Public method to get current weapon index
    public int GetCurrentWeaponIndex()
    {
        return currentWeaponIndex;
    }
    
    void PlayWeaponSwitchSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound("weaponSwitch");
        }
    }
} 