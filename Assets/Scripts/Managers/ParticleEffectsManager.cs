using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Centralized particle effects management system for the Duck Game clone
/// Uses Unity's particle system with burst emissions for optimal performance
/// Based on Unity best practices for particle effects
/// </summary>
public class ParticleEffectsManager : MonoBehaviour
{
    [Header("Particle Prefabs")]
    [SerializeField] private GameObject jumpDustPrefab;
    [SerializeField] private GameObject coinSparklePrefab;
    [SerializeField] private GameObject damageParticlePrefab;
    [SerializeField] private GameObject landingDustPrefab;
    
    [Header("Particle Settings")]
    [SerializeField] private bool particlesEnabled = true;
    
    // Singleton pattern for easy access
    public static ParticleEffectsManager Instance { get; private set; }
    
    // Object pooling for better performance
    private Queue<GameObject> jumpDustPool = new Queue<GameObject>();
    private Queue<GameObject> coinSparklePool = new Queue<GameObject>();
    private Queue<GameObject> damageParticlePool = new Queue<GameObject>();
    private Queue<GameObject> landingDustPool = new Queue<GameObject>();
    
    [Header("Pool Settings")]
    [SerializeField] private int poolSize = 10; // How many particles to pre-instantiate
    
    void Awake()
    {
        // Singleton pattern setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeParticleManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Initializes the particle manager and sets up object pools
    /// </summary>
    private void InitializeParticleManager()
    {
        // Pre-populate object pools for better performance
        if (jumpDustPrefab != null)
            PopulatePool(jumpDustPool, jumpDustPrefab, "JumpDust");
            
        if (coinSparklePrefab != null)
            PopulatePool(coinSparklePool, coinSparklePrefab, "CoinSparkle");
            
        if (damageParticlePrefab != null)
            PopulatePool(damageParticlePool, damageParticlePrefab, "DamageParticle");
            
        if (landingDustPrefab != null)
            PopulatePool(landingDustPool, landingDustPrefab, "LandingDust");
    }
    
    /// <summary>
    /// Populates an object pool with pre-instantiated particles
    /// </summary>
    private void PopulatePool(Queue<GameObject> pool, GameObject prefab, string poolName)
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject particle = Instantiate(prefab, transform);
            particle.name = $"{poolName}_{i}";
            particle.SetActive(false);
            pool.Enqueue(particle);
        }
    }
    
    /// <summary>
    /// Gets a particle from the pool or creates a new one
    /// </summary>
    private GameObject GetParticleFromPool(Queue<GameObject> pool, GameObject prefab, string poolName)
    {
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }
        else
        {
            // Create new particle if pool is empty
            GameObject particle = Instantiate(prefab, transform);
            particle.name = $"{poolName}_New";
            return particle;
        }
    }
    
    /// <summary>
    /// Returns a particle to the pool
    /// </summary>
    private void ReturnParticleToPool(Queue<GameObject> pool, GameObject particle)
    {
        if (particle != null)
        {
            particle.SetActive(false);
            particle.transform.SetParent(transform);
            pool.Enqueue(particle);
        }
    }
    
    /// <summary>
    /// Plays jump dust particles at the specified position
    /// </summary>
    public void PlayJumpDust(Vector3 position)
    {
        if (!particlesEnabled || jumpDustPrefab == null) return;
        
        GameObject particle = GetParticleFromPool(jumpDustPool, jumpDustPrefab, "JumpDust");
        particle.transform.position = position;
        particle.SetActive(true);
        
        // Get the particle system and play it
        ParticleSystem ps = particle.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
            StartCoroutine(ReturnToPoolAfterLifetime(jumpDustPool, particle, ps.main.duration));
        }
    }
    
    /// <summary>
    /// Plays coin sparkle particles at the specified position
    /// </summary>
    public void PlayCoinSparkle(Vector3 position)
    {
        if (!particlesEnabled || coinSparklePrefab == null) return;
        
        GameObject particle = GetParticleFromPool(coinSparklePool, coinSparklePrefab, "CoinSparkle");
        particle.transform.position = position;
        particle.SetActive(true);
        
        // Get the particle system and play it
        ParticleSystem ps = particle.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
            StartCoroutine(ReturnToPoolAfterLifetime(coinSparklePool, particle, ps.main.duration));
        }
    }
    
    /// <summary>
    /// Plays damage particles at the specified position
    /// </summary>
    public void PlayDamageParticles(Vector3 position)
    {
        if (!particlesEnabled || damageParticlePrefab == null) return;
        
        GameObject particle = GetParticleFromPool(damageParticlePool, damageParticlePrefab, "DamageParticle");
        particle.transform.position = position;
        particle.SetActive(true);
        
        // Get the particle system and play it
        ParticleSystem ps = particle.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
            StartCoroutine(ReturnToPoolAfterLifetime(damageParticlePool, particle, ps.main.duration));
        }
    }
    
    /// <summary>
    /// Plays landing dust particles at the specified position
    /// </summary>
    public void PlayLandingDust(Vector3 position)
    {
        if (!particlesEnabled || landingDustPrefab == null) return;
        
        GameObject particle = GetParticleFromPool(landingDustPool, landingDustPrefab, "LandingDust");
        particle.transform.position = position;
        particle.SetActive(true);
        
        // Get the particle system and play it
        ParticleSystem ps = particle.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
            StartCoroutine(ReturnToPoolAfterLifetime(landingDustPool, particle, ps.main.duration));
        }
    }
    
    /// <summary>
    /// Returns a particle to the pool after its lifetime expires
    /// </summary>
    private IEnumerator ReturnToPoolAfterLifetime(Queue<GameObject> pool, GameObject particle, float lifetime)
    {
        yield return new WaitForSeconds(lifetime + 0.1f); // Small buffer
        ReturnParticleToPool(pool, particle);
    }
    
    /// <summary>
    /// Toggles particle effects on/off
    /// </summary>
    public void ToggleParticles()
    {
        particlesEnabled = !particlesEnabled;
    }
    
    /// <summary>
    /// Clears all active particles
    /// </summary>
    public void ClearAllParticles()
    {
        // Stop all particle systems
        ParticleSystem[] allParticles = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in allParticles)
        {
            ps.Stop();
            ps.Clear();
        }
    }
} 