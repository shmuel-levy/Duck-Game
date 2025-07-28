using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int maxEnemies = 5;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float minSpawnDistance = 10f; // Distance from player
    
    [Header("Spawn Area")]
    [SerializeField] private float spawnMinX = -20f;
    [SerializeField] private float spawnMaxX = 20f;
    [SerializeField] private float spawnMinY = -2f;
    [SerializeField] private float spawnMaxY = 2f;
    
    [Header("Enemy Types")]
    [SerializeField] private bool spawnPatrolEnemies = true;
    [SerializeField] private bool spawnChaseEnemies = true;
    
    // Private variables
    private float spawnTimer = 0f;
    private int currentEnemyCount = 0;
    private Transform player;

    void Start()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        // Create enemy prefab if not assigned
        if (enemyPrefab == null)
        {
            CreateEnemyPrefab();
        }
    }

    void Update()
    {
        if (currentEnemyCount < maxEnemies)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                SpawnEnemy();
                spawnTimer = 0f;
            }
        }
    }
    
    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;
        
        Vector3 spawnPosition = GetValidSpawnPosition();
        if (spawnPosition != Vector3.zero)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            currentEnemyCount++;
            
            // Set enemy type
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                // Randomly choose enemy type
                bool isPatrolEnemy = Random.Range(0, 2) == 0;
                if (isPatrolEnemy && spawnPatrolEnemies)
                {
                    // Patrol enemy - already set by default
                }
                else if (spawnChaseEnemies)
                {
                    // Chase enemy - disable patrol
                    enemyScript.SetPatrol(false);
                }
            }
            
            // Subscribe to enemy death
            StartCoroutine(MonitorEnemy(enemy));
        }
    }
    
    Vector3 GetValidSpawnPosition()
    {
        int attempts = 0;
        const int maxAttempts = 20;
        
        while (attempts < maxAttempts)
        {
            // Random position in spawn area
            float x = Random.Range(spawnMinX, spawnMaxX);
            float y = Random.Range(spawnMinY, spawnMaxY);
            Vector3 position = new Vector3(x, y, 0);
            
            // Check distance from player
            if (player != null)
            {
                float distanceToPlayer = Vector2.Distance(position, player.position);
                if (distanceToPlayer < minSpawnDistance)
                {
                    attempts++;
                    continue;
                }
            }
            
            // Check if position is on ground
            if (IsPositionOnGround(position))
            {
                // Check if position is clear (no other enemies nearby)
                Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 2f);
                bool positionClear = true;
                foreach (Collider2D collider in colliders)
                {
                    if (collider.CompareTag("Enemy") || collider.CompareTag("Player"))
                    {
                        positionClear = false;
                        break;
                    }
                }
                
                if (positionClear)
                {
                    return position;
                }
            }
            
            attempts++;
        }
        
        return Vector3.zero; // No valid position found
    }
    
    bool IsPositionOnGround(Vector3 position)
    {
        // Cast a ray downward to check for ground
        RaycastHit2D hit = Physics2D.Raycast(
            position + Vector3.up * 0.5f,  // Start slightly above
            Vector2.down,                   // Cast downward
            2f,                            // Distance to check
            LayerMask.GetMask("Ground")    // Only check Ground layer
        );
        
        return hit.collider != null;
    }
    
    System.Collections.IEnumerator MonitorEnemy(GameObject enemy)
    {
        while (enemy != null)
        {
            yield return new WaitForSeconds(0.5f);
        }
        
        // Enemy was destroyed
        currentEnemyCount--;
    }
    
    void CreateEnemyPrefab()
    {
        // Create a basic enemy prefab
        GameObject enemy = new GameObject("Enemy");
        
        // Add components
        enemy.AddComponent<SpriteRenderer>();
        Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0; // No gravity for enemies
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent rotation
        
        BoxCollider2D collider = enemy.AddComponent<BoxCollider2D>();
        collider.isTrigger = true; // Make it a trigger for damage
        
        enemy.AddComponent<Enemy>();
        
        // Set tag
        enemy.tag = "Enemy";
        
        // Set as prefab
        enemyPrefab = enemy;
        
        Debug.Log("Created enemy prefab. You can now assign a sprite to it.");
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw spawn area
        Gizmos.color = Color.green;
        Vector3 center = new Vector3((spawnMinX + spawnMaxX) / 2, (spawnMinY + spawnMaxY) / 2, 0);
        Vector3 size = new Vector3(spawnMaxX - spawnMinX, spawnMaxY - spawnMinY, 0);
        Gizmos.DrawWireCube(center, size);
        
        // Draw player spawn distance
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, minSpawnDistance);
        }
    }
} 