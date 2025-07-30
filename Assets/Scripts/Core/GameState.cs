using System;
using UnityEngine;

namespace DuckGame.Core
{
    /// <summary>
    /// Core game state and data structures
    /// </summary>
    [Serializable]
    public class GameState
    {
        public int currentScore;
        public int highScore;
        public int currentHealth;
        public int maxHealth;
        public bool isGameActive;
        public bool isPaused;
        
        public GameState()
        {
            currentScore = 0;
            highScore = PlayerPrefs.GetInt("HighScore", 0);
            currentHealth = 3;
            maxHealth = 3;
            isGameActive = true;
            isPaused = false;
        }
    }
    
    /// <summary>
    /// Player data structure
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        public Vector3 position;
        public int health;
        public int maxHealth;
        public int jumpsRemaining;
        public bool isGrounded;
        public bool isInvincible;
        
        public PlayerData()
        {
            position = Vector3.zero;
            health = 3;
            maxHealth = 3;
            jumpsRemaining = 2;
            isGrounded = false;
            isInvincible = false;
        }
    }
    
    /// <summary>
    /// Weapon data structure
    /// </summary>
    [Serializable]
    public class WeaponData
    {
        public string weaponType;
        public int ammo;
        public int maxAmmo;
        public float fireRate;
        public float reloadTime;
        
        public WeaponData(string type, int maxAmmo, float fireRate, float reloadTime)
        {
            this.weaponType = type;
            this.ammo = maxAmmo;
            this.maxAmmo = maxAmmo;
            this.fireRate = fireRate;
            this.reloadTime = reloadTime;
        }
    }
} 