using UnityEngine;
using StarterAssets;
using UnityEngine.Audio;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; 
#endif
public class LeafblowerParticles : MonoBehaviour
{
    [Header("Particle Settings")]
    [Tooltip("Particle system that represents the air or leaves coming out of the blower nozzle.")]
    public ParticleSystem blowerParticles;

    [Header("Audio (Optional)")]
    [Tooltip("Optional looping sound while blowing.")]
    public AudioSource blowerSound;

    [Header("Input Settings")]
    public bool useInputSystem = true; // set false if you use old Input.GetButton

    private StarterAssetsInputs input;
    public AudioMixerGroup sfxMixerGroup;
    void Start()
    {
        if (blowerParticles == null)
        {
            Debug.LogWarning("[LeafblowerParticles] No particle system assigned!");
        }

        if (blowerSound != null && sfxMixerGroup != null)
        {
            blowerSound.outputAudioMixerGroup = sfxMixerGroup;
        }

        if (useInputSystem)
            input = FindObjectOfType<StarterAssetsInputs>();
    }

    void Update()
    {
        bool isPressed = false;

        if (useInputSystem && input != null)
        {
            // New Input System (Starter Assets)
            isPressed = Mouse.current != null && Mouse.current.leftButton.isPressed;
        }
        else
        {
            // Old Input System fallback
            isPressed = Input.GetMouseButton(0);
        }

        if (isPressed)
            StartBlowing();
        else
            StopBlowing();
    }

    private void StartBlowing()
    {
        if (blowerParticles != null && !blowerParticles.isEmitting)
            blowerParticles.Play();

        if (blowerSound != null && !blowerSound.isPlaying)
            blowerSound.Play();
    }

    private void StopBlowing()
    {
        if (blowerParticles != null && blowerParticles.isEmitting)
            blowerParticles.Stop();

        if (blowerSound != null && blowerSound.isPlaying)
            blowerSound.Stop();
    }
}
