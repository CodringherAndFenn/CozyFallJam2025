using UnityEngine;
using StarterAssets;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerFootstepAudio : MonoBehaviour
{
    [Header("Footstep Settings")]
    [Tooltip("Add as many footstep sounds as you want.")]
    public AudioClip[] footstepClips;
    public float stepInterval = 0.5f;       // seconds between steps while walking
    public float sprintMultiplier = 0.7f;   // smaller = faster steps when sprinting

    [Header("Jump & Land Sounds")]
    public AudioClip jumpClip;
    public AudioClip landClip;

    [Header("Audio Settings")]
    [Range(0f, 1f)] public float volume = 0.6f;
    [Range(0.8f, 1.2f)] public float pitchVariation = 0.1f; // random pitch offset ±

    private AudioSource source;
    private CharacterController controller;
    private StarterAssetsInputs input;

    private bool wasGrounded;
    private float stepTimer;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();
        input = GetComponent<StarterAssetsInputs>();

        source.playOnAwake = false;
        source.loop = false;
        wasGrounded = controller.isGrounded;
    }

    void Update()
    {
        HandleFootsteps();
        HandleJumpLand();
    }

    void HandleFootsteps()
    {
        if (!controller.isGrounded || controller.velocity.magnitude < 0.2f)
        {
            stepTimer = 0f;
            return;
        }

        stepTimer += Time.deltaTime;
        float interval = (input != null && input.sprint) ? stepInterval * sprintMultiplier : stepInterval;

        if (stepTimer >= interval)
        {
            PlayRandomFootstep();
            stepTimer = 0f;
        }
    }

    void HandleJumpLand()
    {
        bool grounded = controller.isGrounded;

        // Jump sound when leaving ground
        if (wasGrounded && !grounded)
        {
            PlaySound(jumpClip);
        }

        // Landing sound when touching ground again
        if (!wasGrounded && grounded)
        {
            PlaySound(landClip);
            stepTimer = 0f; // reset step timing after landing
        }

        wasGrounded = grounded;
    }

    void PlayRandomFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0) return;

        int index = Random.Range(0, footstepClips.Length);
        var clip = footstepClips[index];

        // randomize pitch slightly for natural variation
        source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        source.PlayOneShot(clip, volume);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip == null) return;
        source.pitch = 1f;
        source.PlayOneShot(clip, volume);
    }
}
