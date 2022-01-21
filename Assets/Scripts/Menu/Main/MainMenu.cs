using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class <c>MainMenu</c> to handle common menu functions.
/// </summary>
public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Used in the splash screen on "Load Game" to load the game view.
    /// </summary>
    public void PlayGame()
    {
        Debug.Log("Load Scene NR.:" + (SceneManager.GetActiveScene().buildIndex + 1));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// Used in globally to quit the game.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
