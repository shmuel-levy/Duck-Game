using UnityEngine;

public class WeaponFlipper : MonoBehaviour
{
    private Transform weaponSprite;
    private SpriteRenderer weaponRenderer;
    private Transform firePoint;

    void Start()
    {
        weaponSprite = transform.Find("WeaponHolder/WeaponSprite");
        firePoint = transform.Find("WeaponHolder/FirePoint");
        if (weaponSprite == null)
            Debug.LogError("WeaponSprite not found! Check your hierarchy: Duck > WeaponHolder > WeaponSprite");
        if (firePoint == null)
            Debug.LogError("FirePoint not found! Check your hierarchy: Duck > WeaponHolder > FirePoint");
        weaponRenderer = weaponSprite != null ? weaponSprite.GetComponent<SpriteRenderer>() : null;
        if (weaponRenderer == null)
            Debug.LogError("WeaponSprite has no SpriteRenderer! Add one and assign a sprite.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        // Rotate weapon to face the same direction as the duck
        if (weaponSprite != null)
        {
            if (transform.localScale.x < 0)
            {
                // Facing left - rotate weapon to face left
                weaponSprite.localRotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                // Facing right - rotate weapon to face right
                weaponSprite.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
} 