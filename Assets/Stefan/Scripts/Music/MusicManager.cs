using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Music Tracks")]
    [Tooltip("Add several background tracks here; one will loop at a time.")]
    public List<AudioClip> tracks = new List<AudioClip>();

    [Header("Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Tooltip("Speed multiplier while inspecting items.")]
    public float inspectPitch = 0.7f;
    [Tooltip("Seconds to blend pitch changes.")]
    public float pitchFadeDuration = 1.0f;
    [Tooltip("If true, choose random track order; otherwise play sequentially.")]
    public bool shuffle = true;

    private AudioSource source;
    private int currentTrackIndex = 0;

    public AudioMixerGroup musicMixerGroup;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        source = GetComponent<AudioSource>();
        source.outputAudioMixerGroup = musicMixerGroup;
        source.loop = false;
        source.playOnAwake = false;
        source.volume = musicVolume;
        source.pitch = 1f;
    }

    void Start()
    {
        if (tracks.Count > 0)
        {
            StartCoroutine(PlayLoopedTracks());
        }
        else
        {
            Debug.LogWarning("[MusicManager] No tracks assigned!");
        }
    }

    IEnumerator PlayLoopedTracks()
    {
        while (true)
        {
            if (!source.isPlaying)
            {
                if (shuffle)
                    currentTrackIndex = Random.Range(0, tracks.Count);
                else
                    currentTrackIndex = (currentTrackIndex + 1) % tracks.Count;

                source.clip = tracks[currentTrackIndex];
                source.volume = musicVolume;
                source.pitch = 1f;
                source.Play();
            }
            yield return null;
        }
    }

    public void SlowForInspect()
    {
        StopCoroutine(nameof(ChangePitchSmoothly));
        StartCoroutine(ChangePitchSmoothly(inspectPitch));
    }

    public void ResumeNormal()
    {
        StopCoroutine(nameof(ChangePitchSmoothly));
        StartCoroutine(ChangePitchSmoothly(1f));
    }

    IEnumerator ChangePitchSmoothly(float targetPitch)
    {
        float startPitch = source.pitch;
        float time = 0f;

        while (time < pitchFadeDuration)
        {
            time += Time.unscaledDeltaTime; 
            source.pitch = Mathf.Lerp(startPitch, targetPitch, time / pitchFadeDuration);
            yield return null;
        }

        source.pitch = targetPitch;
    }
}
