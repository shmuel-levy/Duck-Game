using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int damage = 1;
    
    [Header("Movement")]
    [SerializeField] private bool patrols = true;
    [SerializeField] private float patrolDistance = 3f;
    [SerializeField] private float waitTime = 1f;
    
    [Header("Visual Effects")]
    [SerializeField] private Color enemyColor = Color.red;
    [SerializeField] private GameObject deathEffect;
    
    // Private variables
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private int currentHealth;
    private bool isDead = false;
    
    // Patrol variables
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool movingRight = true;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    
    // State machine
    private enum EnemyState { Patrol, Chase, Attack, Dead }
    private EnemyState currentState = EnemyState.Patrol;

    void Start()
    {
        // Get components
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Set enemy color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = enemyColor;
        }
        
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        // Initialize
        currentHealth = maxHealth;
        startPosition = transform.position;
        SetNextPatrolTarget();
        
        // Debug info
        Debug.Log($"Enemy spawned at {transform.position}. Patrols: {patrols}");
    }

    void Update()
    {
        if (isDead) return;
        
        // Update state machine
        UpdateState();
        
        // Handle current state
        switch (currentState)
        {
            case EnemyState.Patrol:
                HandlePatrol();
                break;
            case EnemyState.Chase:
                HandleChase();
                break;
            case EnemyState.Attack:
                HandleAttack();
                break;
        }
    }
    
    void UpdateState()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
        }
        else if (distanceToPlayer <= detectionRange)
        {
            currentState = EnemyState.Chase;
        }
        else
        {
            currentState = EnemyState.Patrol;
        }
    }
    
    void HandlePatrol()
    {
        if (!patrols) return;
        
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false;
                SetNextPatrolTarget();
            }
            return;
        }
        
        // Move towards target
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (rb != null)
        {
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        }
        
        // Flip sprite based on direction
        if (direction.x > 0.1f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x < -0.1f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        
        // Check if reached target
        if (Vector2.Distance(transform.position, targetPosition) < 0.5f)
        {
            isWaiting = true;
            waitTimer = waitTime;
        }
    }
    
    void HandleChase()
    {
        if (player == null) return;
        
        // Move towards player
        Vector3 direction = (player.position - transform.position).normalized;
        if (rb != null)
        {
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        }
        
        // Flip sprite based on direction
        if (direction.x > 0.1f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x < -0.1f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    
    void HandleAttack()
    {
        // Stop moving
        if (rb != null)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        
        // Attack logic here (could spawn projectiles, melee attack, etc.)
        // For now, just damage player on collision
    }
    
    void SetNextPatrolTarget()
    {
        if (movingRight)
        {
            targetPosition = startPosition + Vector3.right * patrolDistance;
        }
        else
        {
            targetPosition = startPosition + Vector3.left * patrolDistance;
        }
        movingRight = !movingRight;
        
        Debug.Log($"Enemy patrol target set to: {targetPosition}");
    }
    
    public void SetPatrol(bool shouldPatrol)
    {
        patrols = shouldPatrol;
        Debug.Log($"Enemy patrol set to: {patrols}");
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage. Health: {currentHealth}");
        
        // Flash red
        StartCoroutine(FlashRed());
        
        // Play damage sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound("damage");
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    System.Collections.IEnumerator FlashRed()
    {
        if (spriteRenderer == null) yield break;
        
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.white;
        
        yield return new WaitForSeconds(0.1f);
        
        spriteRenderer.color = originalColor;
    }
    
    void Die()
    {
        isDead = true;
        Debug.Log("Enemy died!");
        
        // Play death sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound("death");
        }
        
        // Spawn death effect
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        
        // Disable components
        if (rb != null) rb.simulated = false;
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        
        // Destroy after delay
        Destroy(gameObject, 0.5f);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DuckController player = other.GetComponent<DuckController>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log($"Enemy damaged player for {damage} damage!");
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw patrol path
        if (patrols)
        {
            Gizmos.color = Color.blue;
            Vector3 start = Application.isPlaying ? startPosition : transform.position;
            Gizmos.DrawLine(start, start + Vector3.right * patrolDistance);
            Gizmos.DrawLine(start, start + Vector3.left * patrolDistance);
        }
    }
} 