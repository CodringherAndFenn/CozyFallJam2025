using UnityEngine;
using UnityEngine.Audio;
public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public void SetFullscreen(bool isFullscreen)
    {
       Screen.fullScreen = isFullscreen;
       Debug.Log("Sanity check: fullscreen mode is" + isFullscreen);
    }

    public void SetVolumeMusic(float volume)
    {
        //audioMixer.SetFloat("MusicVolume", volume);
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("MusicVolume", dB);
        
    }
    public void SetVolumeSFX(float volume)
    {
        //audioMixer.SetFloat("MusicVolume", volume);
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("SoundsVolume", dB);
        
    }
}
