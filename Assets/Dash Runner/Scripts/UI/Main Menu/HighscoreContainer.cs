using UnityEngine;
using TMPro;

public class HighscoreContainer : MonoBehaviour
{
    /// <summary>
    /// Reference to the Text component displaying the highscore.
    /// </summary>
    public TextMeshProUGUI highscoreText;

    void Start()
    {
        float highscore = PlayerPrefs.GetFloat("record", 0);

        highscoreText.text = highscore.ToString("N00").Replace(',', ' ');
    }
}
