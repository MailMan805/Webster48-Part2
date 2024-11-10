using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource jesterAudio;
    public AudioSource guardAudio;
    public AudioSource sfxSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.5f;
    [Range(0f, 1f)]
    public float jesterVolume = 0.5f;
    [Range(0f, 1f)]
    public float guardVolume = 0.5f;

    [Header("Audio Clips")]
    public AudioClip[] musicClips;
    public AudioClip[] sfxClips;
    public AudioClip[] jesterClips;
    public AudioClip[] guardClips;

    private Dictionary<string, AudioClip> musicDictionary;
    private Dictionary<string, AudioClip> sfxDictionary;
    private Dictionary<string, AudioClip> jesterDictionary;
    private Dictionary<string, AudioClip> guardDictionary;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize dictionaries for easy clip lookup
        musicDictionary = new Dictionary<string, AudioClip>();
        sfxDictionary = new Dictionary<string, AudioClip>();

        foreach (var clip in musicClips)
        {
            musicDictionary[clip.name] = clip;
        }

        foreach (var clip in sfxClips)
        {
            sfxDictionary[clip.name] = clip;
        }

        // Set initial volume levels
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
        jesterAudio.volume = jesterVolume;
        guardAudio.volume = guardVolume;
    }

    public void PlayMusic(string clipName)
    {
        if (musicDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Music clip '{clipName}' not found!");
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFX(string clipName)
    {
        if (sfxDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
        else
        {
            Debug.LogWarning($"SFX clip '{clipName}' not found!");
        }
    }

    public void PlayJester(string clipName)
    {
        if (jesterDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            jesterAudio.clip = clip;
            jesterAudio.loop = true;
            jesterAudio.Play();
        }
        else
        {
            Debug.LogWarning($"Jester clip '{clipName}' not found!");
        }
    }

    public void StopJester()
    {
        jesterAudio.Stop();
    }

    public void PlayGuard(string clipName)
    {
        if (guardDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            guardAudio.clip = clip;
            guardAudio.loop = true;
            guardAudio.Play();
        }
        else
        {
            Debug.LogWarning($"Jester clip '{clipName}' not found!");
        }
    }

    public void StopGuard()
    {
        guardAudio.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
    }

    public bool IsMusicPlaying()
    {
        return musicSource.isPlaying;
    }
}
