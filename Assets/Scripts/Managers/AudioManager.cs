using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DuckGame.Core;

namespace DuckGame.Managers
{
    /// <summary>
    /// Centralized audio management system using event-driven architecture
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        
        [Header("Background Music")]
        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField] private float musicVolume = 0.3f;
        
        [Header("Sound Effects")]
        [SerializeField] private AudioClip jumpSound;
        [SerializeField] private AudioClip coinCollectSound;
        [SerializeField] private AudioClip damageSound;
        [SerializeField] private AudioClip deathSound;
        [SerializeField] private AudioClip landSound;
        
        [Header("Weapon Sounds")]
        [SerializeField] private AudioClip pistolFire;
        [SerializeField] private AudioClip shotgunFire;
        [SerializeField] private AudioClip machineGunFire;
        [SerializeField] private AudioClip sniperFire;
        [SerializeField] private AudioClip weaponReload;
        [SerializeField] private AudioClip emptyGun;
        [SerializeField] private AudioClip weaponSwitch;
        
        [Header("Audio Settings")]
        [SerializeField] private float sfxVolume = 0.7f;
        [SerializeField] private bool musicEnabled = true;
        [SerializeField] private bool sfxEnabled = true;
        
        // Singleton pattern
        public static AudioManager Instance { get; private set; }
        
        // Audio clip dictionary for easy access
        private Dictionary<string, AudioClip> audioClips;
        
        void Awake()
        {
            // Singleton pattern setup
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioManager();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void Start()
        {
            // Subscribe to events
            SubscribeToEvents();
            
            // Start background music
            if (musicEnabled && backgroundMusic != null)
            {
                PlayBackgroundMusic();
            }
        }
        
        void OnDestroy()
        {
            // Unsubscribe from events
            UnsubscribeFromEvents();
        }
        
        /// <summary>
        /// Subscribes to audio events
        /// </summary>
        private void SubscribeToEvents()
        {
            GameEvents.OnPlaySound.AddListener(OnPlaySound);
            GameEvents.OnMusicToggle.AddListener(OnMusicToggle);
            GameEvents.OnSFXToggle.AddListener(OnSFXToggle);
            GameEvents.OnPlayerJump.AddListener(OnPlayerJump);
            GameEvents.OnPlayerLand.AddListener(OnPlayerLand);
            GameEvents.OnPlayerTakeDamage.AddListener(OnPlayerTakeDamage);
            GameEvents.OnPlayerDeath.AddListener(OnPlayerDeath);
            GameEvents.OnWeaponFire.AddListener(OnWeaponFire);
            GameEvents.OnWeaponSwitch.AddListener(OnWeaponSwitch);
        }
        
        /// <summary>
        /// Unsubscribes from audio events
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            GameEvents.OnPlaySound.RemoveListener(OnPlaySound);
            GameEvents.OnMusicToggle.RemoveListener(OnMusicToggle);
            GameEvents.OnSFXToggle.RemoveListener(OnSFXToggle);
            GameEvents.OnPlayerJump.RemoveListener(OnPlayerJump);
            GameEvents.OnPlayerLand.RemoveListener(OnPlayerLand);
            GameEvents.OnPlayerTakeDamage.RemoveListener(OnPlayerTakeDamage);
            GameEvents.OnPlayerDeath.RemoveListener(OnPlayerDeath);
            GameEvents.OnWeaponFire.RemoveListener(OnWeaponFire);
            GameEvents.OnWeaponSwitch.RemoveListener(OnWeaponSwitch);
        }
        
        /// <summary>
        /// Initializes the audio manager and sets up audio sources
        /// </summary>
        private void InitializeAudioManager()
        {
            // Create audio sources if they don't exist
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.volume = musicVolume;
            }
            
            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.volume = sfxVolume;
            }
            
            // Initialize audio clips dictionary
            InitializeAudioClips();
        }
        
        /// <summary>
        /// Initializes the dictionary of audio clips for easy access
        /// </summary>
        private void InitializeAudioClips()
        {
            audioClips = new Dictionary<string, AudioClip>
            {
                { "jump", jumpSound },
                { "coin", coinCollectSound },
                { "damage", damageSound },
                { "death", deathSound },
                { "land", landSound },
                { "pistolFire", pistolFire },
                { "shotgunFire", shotgunFire },
                { "machineGunFire", machineGunFire },
                { "sniperFire", sniperFire },
                { "weaponReload", weaponReload },
                { "emptyGun", emptyGun },
                { "weaponSwitch", weaponSwitch }
            };
        }
        
        /// <summary>
        /// Handles play sound events
        /// </summary>
        private void OnPlaySound(string soundName)
        {
            PlaySound(soundName);
        }
        
        /// <summary>
        /// Handles music toggle events
        /// </summary>
        private void OnMusicToggle()
        {
            ToggleMusic();
        }
        
        /// <summary>
        /// Handles SFX toggle events
        /// </summary>
        private void OnSFXToggle()
        {
            ToggleSFX();
        }
        
        /// <summary>
        /// Handles player jump events
        /// </summary>
        private void OnPlayerJump()
        {
            PlayJumpSound();
        }
        
        /// <summary>
        /// Handles player land events
        /// </summary>
        private void OnPlayerLand()
        {
            PlayLandSound();
        }
        
        /// <summary>
        /// Handles player take damage events
        /// </summary>
        private void OnPlayerTakeDamage()
        {
            PlayDamageSound();
        }
        
        /// <summary>
        /// Handles player death events
        /// </summary>
        private void OnPlayerDeath()
        {
            PlayDeathSound();
        }
        
        /// <summary>
        /// Handles weapon fire events
        /// </summary>
        private void OnWeaponFire()
        {
            // Default weapon fire sound
            PlaySound("pistolFire");
        }
        
        /// <summary>
        /// Handles weapon switch events
        /// </summary>
        private void OnWeaponSwitch(string weaponType)
        {
            PlaySound("weaponSwitch");
        }
        
        /// <summary>
        /// Plays background music
        /// </summary>
        public void PlayBackgroundMusic()
        {
            if (!musicEnabled || backgroundMusic == null) return;
            
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
        
        /// <summary>
        /// Stops background music
        /// </summary>
        public void StopBackgroundMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }
        
        /// <summary>
        /// Plays a sound effect by name
        /// </summary>
        public void PlaySound(string soundName)
        {
            if (!sfxEnabled) return;
            
            if (audioClips.ContainsKey(soundName) && audioClips[soundName] != null)
            {
                sfxSource.PlayOneShot(audioClips[soundName]);
            }
            else
            {
                Debug.LogWarning($"Sound '{soundName}' not found or null!");
            }
        }
        
        /// <summary>
        /// Plays a custom audio clip
        /// </summary>
        public void PlaySound(AudioClip clip)
        {
            if (!sfxEnabled || clip == null) return;
            
            sfxSource.PlayOneShot(clip);
        }
        
        /// <summary>
        /// Gets an audio clip by name
        /// </summary>
        public AudioClip GetAudioClip(string clipName)
        {
            if (audioClips.ContainsKey(clipName))
            {
                return audioClips[clipName];
            }
            Debug.LogWarning($"AudioManager: Clip '{clipName}' not found in dictionary!");
            return null;
        }
        
        /// <summary>
        /// Plays jump sound effect
        /// </summary>
        public void PlayJumpSound()
        {
            PlaySound("jump");
        }
        
        /// <summary>
        /// Plays coin collection sound effect
        /// </summary>
        public void PlayCoinSound()
        {
            PlaySound("coin");
        }
        
        /// <summary>
        /// Plays damage sound effect
        /// </summary>
        public void PlayDamageSound()
        {
            PlaySound("damage");
        }
        
        /// <summary>
        /// Plays death sound effect
        /// </summary>
        public void PlayDeathSound()
        {
            PlaySound("death");
        }
        
        /// <summary>
        /// Plays landing sound effect
        /// </summary>
        public void PlayLandSound()
        {
            PlaySound("land");
        }
        
        /// <summary>
        /// Toggles music on/off
        /// </summary>
        public void ToggleMusic()
        {
            musicEnabled = !musicEnabled;
            if (musicEnabled)
            {
                musicSource.volume = musicVolume;
                if (!musicSource.isPlaying && backgroundMusic != null)
                {
                    PlayBackgroundMusic();
                }
            }
            else
            {
                musicSource.volume = 0f;
            }
        }
        
        /// <summary>
        /// Toggles sound effects on/off
        /// </summary>
        public void ToggleSFX()
        {
            sfxEnabled = !sfxEnabled;
        }
        
        /// <summary>
        /// Sets music volume
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
            {
                musicSource.volume = musicVolume;
            }
        }
        
        /// <summary>
        /// Sets SFX volume
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            if (sfxSource != null)
            {
                sfxSource.volume = sfxVolume;
            }
        }
    }
} 