using UnityEngine;

public class BulletSelfDestruct : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float lifeTime = 2f;
    public int damage = 1;
    
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we hit an enemy
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                
                // Play hit sound
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySound("damage");
                }
                
                // Destroy bullet
                Destroy(gameObject);
            }
        }
        // Check if we hit the ground or other obstacles
        else if (other.CompareTag("Ground") || other.CompareTag("Hazard"))
        {
            // Play impact sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound("thud");
            }
            
            // Destroy bullet
            Destroy(gameObject);
        }
    }
} 