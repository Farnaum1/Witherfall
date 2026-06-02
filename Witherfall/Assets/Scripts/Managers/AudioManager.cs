using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSourceOneshot;
    [SerializeField] private AudioSource sfxSourceLoop;
    [SerializeField] private AudioSource musicSource;

    [Header("Music Clips")]
    public AudioClip mainMusic;
    public AudioClip secretAreaMusic;

    [Header("Music Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.25f;

    [Header("Player SFX")]
    [SerializeField] private AudioClip[] footstepClips;
    public AudioClip playerJump;
    public AudioClip playerDoubleJump;
    public AudioClip playerWallJump;
    public AudioClip playerLand;
    public AudioClip playerHurt;
    public AudioClip playerDeath;

    [Header("Brick SFX")]
    public AudioClip brickHit;
    public AudioClip brickBreak;
    public AudioClip projectileSwoosh;

    [Header("Crate SFX")]
    public AudioClip crateImpactLight;
    public AudioClip crateImpactHeavy;

    [Header("UI SFX")]
    public AudioClip uiConfirm;
    public AudioClip uiCancel;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource != null)
        {
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.volume = musicVolume;
            musicSource.spatialBlend = 0f; // 2D audio
        }
    }

    private void Start()
    {
        PlayMusic(mainMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null)
        {
            Debug.LogError("Music AudioSource is missing on AudioManager.");
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning("Tried to play music, but AudioClip is null.");
            return;
        }

        // Don't restart the same music if it's already playing
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource == null)
            return;

        musicSource.Stop();
    }

    public void PauseMusic()
    {
        if (musicSource == null)
            return;

        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        if (musicSource == null)
            return;

        musicSource.UnPause();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);

        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    public void PlayMusicWithFade(AudioClip clip, float fadeTime = 1f)
    {
        if (musicSource == null)
        {
            Debug.LogError("Music AudioSource is missing on AudioManager.");
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning("Tried to fade music, but AudioClip is null.");
            return;
        }

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeToMusic(clip, fadeTime));
    }

    private IEnumerator FadeToMusic(AudioClip newClip, float fadeTime)
    {
        float startVolume = musicSource.volume;

        // Fade out current music
        while (musicSource.volume > 0f)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.loop = true;
        musicSource.Play();

        // Fade in new music
        while (musicSource.volume < musicVolume)
        {
            musicSource.volume += musicVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        musicSource.volume = musicVolume;
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        sfxSourceOneshot.PlayOneShot(clip, volume);
    }

    public void PlaySFXLoop(AudioClip clip, float volume = 1f, float minPitch = 0.9f, float maxPitch = 1.1f)
    {
        if (clip == null) return;

        sfxSourceLoop.clip = clip;
        sfxSourceLoop.volume = volume;
        sfxSourceLoop.loop = true;
        sfxSourceLoop.pitch = Random.Range(minPitch, maxPitch);
        sfxSourceLoop.Play(); ;
    }

    public void StopSFXLoop()
    {
        if (sfxSourceLoop == null) return;

        sfxSourceLoop.Stop();
        sfxSourceLoop.clip = null;
    }

    public void PlaySFXRandomPitch(AudioClip clip, float volume = 1f, float minPitch = 0.9f, float maxPitch = 1.1f)
    {
        if (clip == null) return;

        sfxSourceOneshot.pitch = Random.Range(minPitch, maxPitch);
        sfxSourceOneshot.PlayOneShot(clip, volume);
    }

    public void PlayFootstep()
    {
        if (footstepClips.Length == 0) return;

        int randomIndex = Random.Range(0, footstepClips.Length);
        AudioClip clipToPlay = footstepClips[randomIndex];
        sfxSourceLoop.pitch = Random.Range(0.9f, 1.1f);
        sfxSourceLoop.PlayOneShot(clipToPlay);
        sfxSourceLoop.pitch = 1.0f;
    }
}
