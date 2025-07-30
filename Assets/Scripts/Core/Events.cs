using System;
using UnityEngine;
using UnityEngine.Events;

namespace DuckGame.Core
{
    /// <summary>
    /// Centralized event system for the Duck Game
    /// </summary>
    public static class GameEvents
    {
        // Player Events
        public static UnityEvent<int> OnPlayerHealthChanged = new UnityEvent<int>();
        public static UnityEvent<int> OnPlayerScoreChanged = new UnityEvent<int>();
        public static UnityEvent OnPlayerDeath = new UnityEvent();
        public static UnityEvent OnPlayerJump = new UnityEvent();
        public static UnityEvent OnPlayerLand = new UnityEvent();
        public static UnityEvent OnPlayerTakeDamage = new UnityEvent();
        
        // Game State Events
        public static UnityEvent OnGameStart = new UnityEvent();
        public static UnityEvent OnGameOver = new UnityEvent();
        public static UnityEvent OnGamePause = new UnityEvent();
        public static UnityEvent OnGameResume = new UnityEvent();
        public static UnityEvent OnLevelComplete = new UnityEvent();
        
        // Weapon Events
        public static UnityEvent<string> OnWeaponSwitch = new UnityEvent<string>();
        public static UnityEvent OnWeaponFire = new UnityEvent();
        public static UnityEvent OnWeaponReload = new UnityEvent();
        public static UnityEvent OnWeaponEmpty = new UnityEvent();
        
        // Enemy Events
        public static UnityEvent<Enemy> OnEnemyDeath = new UnityEvent<Enemy>();
        public static UnityEvent<Enemy> OnEnemySpawn = new UnityEvent<Enemy>();
        
        // Collectible Events
        public static UnityEvent<Collectible> OnCollectibleCollected = new UnityEvent<Collectible>();
        
        // Audio Events
        public static UnityEvent<string> OnPlaySound = new UnityEvent<string>();
        public static UnityEvent OnMusicToggle = new UnityEvent();
        public static UnityEvent OnSFXToggle = new UnityEvent();
        
        // Particle Events
        public static UnityEvent<Vector3, string> OnPlayParticle = new UnityEvent<Vector3, string>();
    }
    
    /// <summary>
    /// Event data for damage
    /// </summary>
    [Serializable]
    public class DamageEventData
    {
        public GameObject attacker;
        public GameObject target;
        public int damage;
        public Vector3 hitPoint;
        
        public DamageEventData(GameObject attacker, GameObject target, int damage, Vector3 hitPoint)
        {
            this.attacker = attacker;
            this.target = target;
            this.damage = damage;
            this.hitPoint = hitPoint;
        }
    }
    
    /// <summary>
    /// Event data for weapon firing
    /// </summary>
    [Serializable]
    public class WeaponFireEventData
    {
        public string weaponType;
        public Vector3 firePosition;
        public Vector3 fireDirection;
        public int ammoRemaining;
        
        public WeaponFireEventData(string weaponType, Vector3 firePosition, Vector3 fireDirection, int ammoRemaining)
        {
            this.weaponType = weaponType;
            this.firePosition = firePosition;
            this.fireDirection = fireDirection;
            this.ammoRemaining = ammoRemaining;
        }
    }
} 