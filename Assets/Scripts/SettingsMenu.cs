using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public void SetFullscreen(bool isFullscreen)
    {
       Screen.fullScreen = isFullscreen;
       Debug.Log("Sanity check: fullscreen mode is" + isFullscreen);
    }
}
