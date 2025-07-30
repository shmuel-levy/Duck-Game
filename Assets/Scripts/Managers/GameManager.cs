using UnityEngine;
using TMPro;
using DuckGame.Core;
using DuckGame.Controllers;

namespace DuckGame.Managers
{
    /// <summary>
    /// Manages game state, scoring, and UI using event-driven architecture
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private int startingScore = 0;
        
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI healthText;
        
        // Singleton pattern
        public static GameManager Instance { get; private set; }
        
        // Core game state
        private GameState gameState;
        private DuckController player;
        
        void Awake()
        {
            // Singleton pattern setup
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGameManager();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void Start()
        {
            // Find the player
            player = FindObjectOfType<DuckController>();
            if (player == null)
            {
                Debug.LogWarning("GameManager: No DuckController found in scene!");
            }
            
            // Subscribe to events
            SubscribeToEvents();
            
            // Initialize game
            InitializeGame();
        }
        
        void OnDestroy()
        {
            // Unsubscribe from events
            UnsubscribeFromEvents();
        }
        
        /// <summary>
        /// Initializes the game manager
        /// </summary>
        private void InitializeGameManager()
        {
            gameState = new GameState();
        }
        
        /// <summary>
        /// Subscribes to game events
        /// </summary>
        private void SubscribeToEvents()
        {
            GameEvents.OnPlayerHealthChanged.AddListener(OnPlayerHealthChanged);
            GameEvents.OnPlayerScoreChanged.AddListener(OnPlayerScoreChanged);
            GameEvents.OnPlayerDeath.AddListener(OnPlayerDeath);
            GameEvents.OnGameStart.AddListener(OnGameStart);
            GameEvents.OnGameOver.AddListener(OnGameOver);
        }
        
        /// <summary>
        /// Unsubscribes from game events
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            GameEvents.OnPlayerHealthChanged.RemoveListener(OnPlayerHealthChanged);
            GameEvents.OnPlayerScoreChanged.RemoveListener(OnPlayerScoreChanged);
            GameEvents.OnPlayerDeath.RemoveListener(OnPlayerDeath);
            GameEvents.OnGameStart.RemoveListener(OnGameStart);
            GameEvents.OnGameOver.RemoveListener(OnGameOver);
        }
        
        /// <summary>
        /// Initializes the game state
        /// </summary>
        private void InitializeGame()
        {
            gameState.currentScore = startingScore;
            gameState.isGameActive = true;
            gameState.isPaused = false;
            
            // Load high score from PlayerPrefs
            gameState.highScore = PlayerPrefs.GetInt("HighScore", 0);
            
            // Trigger game start event
            GameEvents.OnGameStart.Invoke();
            
            // Update UI
            UpdateUI();
        }
        
        /// <summary>
        /// Handles player health changes
        /// </summary>
        private void OnPlayerHealthChanged(int newHealth)
        {
            gameState.currentHealth = newHealth;
            UpdateUI();
            
            if (newHealth <= 0)
            {
                GameEvents.OnPlayerDeath.Invoke();
            }
        }
        
        /// <summary>
        /// Handles player score changes
        /// </summary>
        private void OnPlayerScoreChanged(int newScore)
        {
            gameState.currentScore = newScore;
            
            // Check for new high score
            if (gameState.currentScore > gameState.highScore)
            {
                gameState.highScore = gameState.currentScore;
                PlayerPrefs.SetInt("HighScore", gameState.highScore);
                PlayerPrefs.Save();
            }
            
            UpdateUI();
        }
        
        /// <summary>
        /// Handles player death
        /// </summary>
        private void OnPlayerDeath()
        {
            gameState.isGameActive = false;
            GameEvents.OnGameOver.Invoke();
        }
        
        /// <summary>
        /// Handles game start
        /// </summary>
        private void OnGameStart()
        {
            Debug.Log("Game started!");
        }
        
        /// <summary>
        /// Handles game over
        /// </summary>
        private void OnGameOver()
        {
            Debug.Log("Game over!");
            // You can add more game over logic here
        }
        
        /// <summary>
        /// Updates all UI elements
        /// </summary>
        private void UpdateUI()
        {
            // Update score text
            if (scoreText != null)
            {
                scoreText.text = $"Score: {gameState.currentScore}";
            }
            
            // Update high score text
            if (highScoreText != null)
            {
                highScoreText.text = $"High Score: {gameState.highScore}";
            }
            
            // Update health text
            if (healthText != null)
            {
                healthText.text = $"Health: {gameState.currentHealth}";
            }
        }
        
        /// <summary>
        /// Adds points to the current score
        /// </summary>
        public void AddScore(int points)
        {
            if (!gameState.isGameActive) return;
            
            int newScore = gameState.currentScore + points;
            GameEvents.OnPlayerScoreChanged.Invoke(newScore);
        }
        
        /// <summary>
        /// Pauses the game
        /// </summary>
        public void PauseGame()
        {
            if (!gameState.isGameActive) return;
            
            gameState.isPaused = true;
            Time.timeScale = 0f;
            GameEvents.OnGamePause.Invoke();
        }
        
        /// <summary>
        /// Resumes the game
        /// </summary>
        public void ResumeGame()
        {
            if (!gameState.isGameActive) return;
            
            gameState.isPaused = false;
            Time.timeScale = 1f;
            GameEvents.OnGameResume.Invoke();
        }
        
        /// <summary>
        /// Restarts the game
        /// </summary>
        public void RestartGame()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
        
        /// <summary>
        /// Gets the current score
        /// </summary>
        public int GetCurrentScore()
        {
            return gameState.currentScore;
        }
        
        /// <summary>
        /// Gets the high score
        /// </summary>
        public int GetHighScore()
        {
            return gameState.highScore;
        }
        
        /// <summary>
        /// Gets the current health
        /// </summary>
        public int GetCurrentHealth()
        {
            return gameState.currentHealth;
        }
        
        /// <summary>
        /// Checks if the game is active
        /// </summary>
        public bool IsGameActive()
        {
            return gameState.isGameActive;
        }
        
        /// <summary>
        /// Checks if the game is paused
        /// </summary>
        public bool IsGamePaused()
        {
            return gameState.isPaused;
        }
    }
} 