using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("VScene"); // <- change to desired level
    }

    public void Quit()
    {
        Debug.Log("Sanity check - Quitting game...");
        Application.Quit();
    }
    
}
