using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Audio.Scripts
{
    public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float masterVolume = 1f;
    
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.7f;
    
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;
    
    [Header("Music Settings")]
    [SerializeField] private float musicFadeDuration = 1f;
    
    [Header("SFX Pooling")]
    [SerializeField] private int sfxSourcePoolSize = 10;
    private List<AudioSource> sfxSourcePool = new List<AudioSource>();
    
    private Coroutine musicFadeCoroutine;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeAudioSources();
        
        CreateSFXPool();
    }
    
    void InitializeAudioSources()
    {
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        
        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
        
        UpdateVolumes();
    }
    
    void CreateSFXPool()
    {
        for (int i = 0; i < sfxSourcePoolSize; i++)
        {
            GameObject sfxObj = new GameObject($"SFXPool_{i}");
            sfxObj.transform.SetParent(transform);
            AudioSource source = sfxObj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            sfxSourcePool.Add(source);
        }
    }
    
    AudioSource GetAvailableSFXSource()
    {
        // Find an available source from pool
        foreach (var source in sfxSourcePool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        
        // If all busy, use the main SFX source
        return sfxSource;
    }
    
    // ===== PLAY SOUND EFFECTS =====
    
    /// <summary>
    /// Play a sound effect using AudioClipData
    /// </summary>
    public void PlaySound(AudioClipData clipData)
    {
        if (clipData == null || clipData.audioClip == null) return;
        
        AudioSource source = GetAvailableSFXSource();
        
        float pitch = clipData.pitch;
        if (clipData.randomizePitch)
        {
            pitch += Random.Range(-clipData.pitchVariation, clipData.pitchVariation);
        }
        
        source.pitch = pitch;
        source.volume = clipData.volume * sfxVolume * masterVolume;
        source.clip = clipData.audioClip;
        source.loop = clipData.loop;
        
        source.Play();
    }
    
    /// <summary>
    /// Play a sound effect using just the AudioClip
    /// </summary>
    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        
        AudioSource source = GetAvailableSFXSource();
        source.pitch = 1f;
        source.volume = volume * sfxVolume * masterVolume;
        source.PlayOneShot(clip);
    }
    
    /// <summary>
    /// Play sound at a specific position in 3D space
    /// </summary>
    public void PlaySoundAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;
        
        AudioSource.PlayClipAtPoint(clip, position, volume * sfxVolume * masterVolume);
    }
    
    // ===== MUSIC CONTROL =====
    
    /// <summary>
    /// Play music with optional fade in
    /// </summary>
    public void PlayMusic(AudioClipData musicData, bool fadeIn = true)
    {
        if (musicData == null || musicData.audioClip == null) return;
        
        if (fadeIn)
        {
            PlayMusicWithFade(musicData.audioClip, musicData.volume);
        }
        else
        {
            musicSource.clip = musicData.audioClip;
            musicSource.volume = musicData.volume * musicVolume * masterVolume;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
    
    /// <summary>
    /// Play music using just AudioClip
    /// </summary>
    public void PlayMusic(AudioClip clip, bool fadeIn = true, float volume = 1f)
    {
        if (clip == null) return;
        
        if (fadeIn)
        {
            PlayMusicWithFade(clip, volume);
        }
        else
        {
            musicSource.clip = clip;
            musicSource.volume = volume * musicVolume * masterVolume;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
    
    void PlayMusicWithFade(AudioClip clip, float targetVolume = 1f)
    {
        if (musicFadeCoroutine != null)
        {
            StopCoroutine(musicFadeCoroutine);
        }
        
        musicFadeCoroutine = StartCoroutine(FadeMusicCoroutine(clip, targetVolume));
    }
    
    IEnumerator FadeMusicCoroutine(AudioClip newClip, float targetVolume)
    {
        // Fade out current music
        float startVolume = musicSource.volume;
        float elapsed = 0f;
        
        while (elapsed < musicFadeDuration / 2f)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / (musicFadeDuration / 2f));
            yield return null;
        }
        
        // Switch to new clip
        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();
        
        // Fade in new music
        elapsed = 0f;
        float finalVolume = targetVolume * musicVolume * masterVolume;
        
        while (elapsed < musicFadeDuration / 2f)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, finalVolume, elapsed / (musicFadeDuration / 2f));
            yield return null;
        }
        
        musicSource.volume = finalVolume;
        musicFadeCoroutine = null;
    }
    
    /// <summary>
    /// Stop music with fade out
    /// </summary>
    public void StopMusic(bool fadeOut = true)
    {
        if (fadeOut)
        {
            if (musicFadeCoroutine != null)
            {
                StopCoroutine(musicFadeCoroutine);
            }
            musicFadeCoroutine = StartCoroutine(FadeOutMusicCoroutine());
        }
        else
        {
            musicSource.Stop();
        }
    }
    
    IEnumerator FadeOutMusicCoroutine()
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;
        
        while (elapsed < musicFadeDuration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / musicFadeDuration);
            yield return null;
        }
        
        musicSource.Stop();
        musicSource.volume = startVolume;
        musicFadeCoroutine = null;
    }
    
    public void PauseMusic()
    {
        musicSource.Pause();
    }
    
    public void ResumeMusic()
    {
        musicSource.UnPause();
    }
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }
    
    void UpdateVolumes()
    {
        if (musicSource != null && musicSource.clip != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }
    }
    
    public void StopAllSounds()
    {
        sfxSource.Stop();
        foreach (var source in sfxSourcePool)
        {
            source.Stop();
        }
    }
    
    public float GetMusicVolume() => musicVolume;
    
    public float GetSFXVolume() => sfxVolume;
    
    public float GetMasterVolume() => masterVolume;

    public bool IsMusicPlaying() => musicSource.isPlaying;
}
}