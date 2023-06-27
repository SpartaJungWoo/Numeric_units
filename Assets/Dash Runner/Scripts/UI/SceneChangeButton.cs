using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeButton : MonoBehaviour
{
    /// <summary>
    /// Name of the scene to load.
    /// </summary>
    public string sceneToLoad;

    /// <summary>
    /// Load the game's scene.
    /// </summary>
    public void LoadScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneToLoad);
    }
}
