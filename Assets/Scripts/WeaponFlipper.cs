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

        if (weaponRenderer != null)
            weaponRenderer.flipX = transform.localScale.x < 0;
    }
} 