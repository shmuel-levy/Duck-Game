using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // For TextMeshPro elements

/// <summary>
/// Manages game state, scoring, and UI
/// Based on Unity best practices for game management
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int startingScore = 0;
    [SerializeField] private int highScore = 0;
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText; // Score display
    [SerializeField] private TextMeshProUGUI highScoreText; // High score display
    [SerializeField] private TextMeshProUGUI healthText; // Health display
    
    [Header("Game State")]
    [SerializeField] private bool isGameActive = true;
    
    // Singleton pattern for easy access
    public static GameManager Instance { get; private set; }
    
    // Private variables
    private int currentScore = 0;
    private DuckController player;
    
    void Awake()
    {
        // Singleton pattern setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Initialize game
        InitializeGame();
        
        // Find the player
        player = FindObjectOfType<DuckController>();
        if (player == null)
        {
            Debug.LogWarning("GameManager: No DuckController found in scene!");
        }
    }
    
    void Update()
    {
        // Update UI
        UpdateUI();
        
        // Check for game over
        CheckGameOver();
    }
    
    /// <summary>
    /// Initializes the game state
    /// </summary>
    private void InitializeGame()
    {
        currentScore = startingScore;
        isGameActive = true;
        
        // Load high score from PlayerPrefs
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        
        Debug.Log("Game initialized!");
    }
    
    /// <summary>
    /// Adds points to the current score
    /// </summary>
    public void AddScore(int points)
    {
        if (!isGameActive) return;
        
        currentScore += points;
        
        // Check for new high score
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            Debug.Log($"New High Score: {highScore}!");
        }
        
        Debug.Log($"Score: {currentScore} (+{points})");
    }
    
    /// <summary>
    /// Updates all UI elements
    /// </summary>
    private void UpdateUI()
    {
        // Update score text
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
        
        // Update high score text
        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: {highScore}";
        }
        
        // Update health text
        if (healthText != null && player != null)
        {
            // We'll need to add a GetHealth method to DuckController
            healthText.text = $"Health: {player.GetCurrentHealth()}";
        }
    }
    
    /// <summary>
    /// Checks if the game should end
    /// </summary>
    private void CheckGameOver()
    {
        if (!isGameActive) return;
        
        // Check if player is dead
        if (player != null && player.GetCurrentHealth() <= 0)
        {
            GameOver();
        }
    }
    
    /// <summary>
    /// Handles game over
    /// </summary>
    private void GameOver()
    {
        isGameActive = false;
        Debug.Log("Game Over!");
        
        // You can add more game over logic here
        // - Show game over screen
        // - Play game over sound
        // - Restart button
    }
    
    /// <summary>
    /// Restarts the game
    /// </summary>
    public void RestartGame()
    {
        // Reload the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
    
    /// <summary>
    /// Gets the current score
    /// </summary>
    public int GetCurrentScore()
    {
        return currentScore;
    }
    
    /// <summary>
    /// Gets the high score
    /// </summary>
    public int GetHighScore()
    {
        return highScore;
    }
    
    /// <summary>
    /// Checks if the game is active
    /// </summary>
    public bool IsGameActive()
    {
        return isGameActive;
    }
} 