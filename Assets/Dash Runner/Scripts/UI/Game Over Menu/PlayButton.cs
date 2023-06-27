using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    /// <summary>
    /// Load the game's scene.
    /// </summary>
    /// <param name="sceneName">Name of the scene to load. By default, "Game".</param>
    public void Play(string sceneName = "Game")
    {
        SceneManager.LoadScene(sceneName);
    }
}
