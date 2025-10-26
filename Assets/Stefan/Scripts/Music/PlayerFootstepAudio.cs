using UnityEngine;
using StarterAssets;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerFootstepAudio : MonoBehaviour
{
    [Header("Footstep Settings")]
    public AudioClip footstepClip;        // one step sound
    public float stepInterval = 0.5f;     // seconds between steps while walking
    public float sprintMultiplier = 0.7f; // faster steps when sprinting

    [Header("Jump & Land Sounds")]
    public AudioClip jumpClip;
    public AudioClip landClip;

    [Header("Volume")]
    [Range(0f, 1f)] public float volume = 0.6f;

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
        // Don't play steps while in air or not moving
        if (!controller.isGrounded || controller.velocity.magnitude < 0.2f)
        {
            stepTimer = 0f;
            return;
        }

        // Count time between steps
        stepTimer += Time.deltaTime;
        float interval = input != null && input.sprint ? stepInterval * sprintMultiplier : stepInterval;

        if (stepTimer >= interval)
        {
            if (footstepClip)
                source.PlayOneShot(footstepClip, volume);
            stepTimer = 0f;
        }
    }

    void HandleJumpLand()
    {
        bool grounded = controller.isGrounded;

        // Detect jump (leaving ground)
        if (wasGrounded && !grounded)
        {
            if (jumpClip)
                source.PlayOneShot(jumpClip, volume);
        }

        // Detect landing (touching ground)
        if (!wasGrounded && grounded)
        {
            if (landClip)
                source.PlayOneShot(landClip, volume);
            stepTimer = 0f; // reset step timer
        }

        wasGrounded = grounded;
    }
}
