using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Centralized audio management system for the Duck Game clone
/// Uses Unity's singleton pattern and AudioSource.PlayOneShot() for optimal performance
/// Based on Unity best practices for audio management
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource; // For background music
    [SerializeField] private AudioSource sfxSource;   // For sound effects
    
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
    
    // Singleton pattern for easy access
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
        
        // Start background music
        if (musicEnabled && backgroundMusic != null)
        {
            PlayBackgroundMusic();
        }
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
    /// <param name="soundName">Name of the sound effect</param>
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
    /// <param name="clip">Audio clip to play</param>
    public void PlaySound(AudioClip clip)
    {
        if (!sfxEnabled || clip == null) return;
        
        sfxSource.PlayOneShot(clip);
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
    /// <param name="volume">Volume level (0-1)</param>
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
    /// <param name="volume">Volume level (0-1)</param>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }
} 