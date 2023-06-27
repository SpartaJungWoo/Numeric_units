using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverMenu : MonoBehaviour
{
    /// <summary>
    /// Reference to the block displayed by default.
    /// </summary>
    public GameObject defaultBlock;

    /// <summary>
    /// Reference to the block with the continue button.
    /// </summary>
    public GameObject continueBlock;

    /// <summary>
    /// Reference to the old score bloc.
    /// </summary>
    public GameObject oldScoreBlock;

    /// <summary>
    /// Reference to the old score text.
    /// </summary>
    public TextMeshProUGUI oldScoreText;

    /// <summary>
    /// Reference to the old score text in the continue block.
    /// </summary>
    public TextMeshProUGUI oldScoreContinueText;

    /// <summary>
    /// Reference to the new record block.
    /// </summary>
    public GameObject newRecordBlock;

    /// <summary>
    /// Reference to the text block to display the score in the retry panel.
    /// </summary>
    public TextMeshProUGUI retryScoreText;

    /// <summary>
    /// Reference to the text block to display the score in the continue panel.
    /// </summary>
    public TextMeshProUGUI continueScoreText;

    /// <summary>
    /// Player's current score.
    /// </summary>
    float playerScore;

    /// <summary>
    /// Player's current highscore.
    /// </summary>
    float playerHighscore;

    /// <summary>
    /// Flag to determine whether the player could have continued or not.
    /// </summary>
    bool couldHaveContinuedAlready = false;

    void Awake()
    {
        couldHaveContinuedAlready = false;
    }

    void OnEnable()
    {
        HideAll();
    }

    /// <summary>
    /// Initialize the Game Over menu.
    /// </summary>
    /// <param name="score">Player's score.</param>
    /// <param name="highscore">Player's highscore.</param>
    public void Init(float score, float highscore)
    {
        playerScore = score;
        playerHighscore = highscore;

        DisplayScore();
        DisplayHighscoreBlock();
        DisplayMainBlock();
    }

    /// <summary>
    /// Hides the currently displayed block and displays the default block.
    /// </summary>
    public void SwitchToDefault()
    {
        HideAll();
        defaultBlock.SetActive(true);
    }

    /// <summary>
    /// Reload the scene.
    /// </summary>
    public void TryAgain()
    {
        couldHaveContinuedAlready = false;
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// Continues the game.
    /// </summary>
    public void Continue()
    {
        MainGameManager.Instance.Continue();
    }

    /// <summary>
    /// Displays the score.
    /// </summary>
    void DisplayScore()
    {
        retryScoreText.text = Mathf.Floor(playerScore).ToString("N00").Replace(',', ' ');
        continueScoreText.text = Mathf.Floor(playerScore).ToString("N00").Replace(',', ' ');
    }

    /// <summary>
    /// Displays the highscore, depending if it is a new highscore or not.
    /// </summary>
    void DisplayHighscoreBlock()
    {
        if (Mathf.Floor(playerScore) > playerHighscore)
        {
            oldScoreBlock.SetActive(false);
            newRecordBlock.SetActive(true);
        }
        else
        {
            oldScoreBlock.SetActive(true);
            newRecordBlock.SetActive(false);
            oldScoreText.text = playerHighscore.ToString("N00").Replace(',', ' ');
        }

        oldScoreContinueText.text = playerHighscore.ToString("N00").Replace(',', ' ');
    }

    /// <summary>
    /// Displays the proper block, according to the context.
    /// </summary>
    void DisplayMainBlock()
    {
        if (CanContinue())
        {
            couldHaveContinuedAlready = true;
            continueBlock.SetActive(true);
        }
        else
        {
            SwitchToDefault();
        }
    }

    /// <summary>
    /// Does the player meet the prerequisite to continue?
    /// </summary>
    /// <returns><c>true</c> if the player can continue, <c>false</c> otherwise.</returns>
    bool CanContinue()
    {
        if (couldHaveContinuedAlready)
        {
            return false;
        }

        int[] scores = GetLastScores();
        UpdateScores(ref scores);

        return playerScore / GetAverage(scores) > 0.8f;
    }

    /// <summary>
    /// Retrieves the last scores of the player, as saved in the PlayerPrefs.
    /// </summary>
    /// <returns>The player's last scores.</returns>
    int[] GetLastScores()
    {
        string[] serializedScores = PlayerPrefs.GetString("last_scores", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        int[] scores = new int[serializedScores.Length];

        for (int i = 0; i < serializedScores.Length; i++)
        {
            scores[i] = int.Parse(serializedScores[i]);
        }

        return scores;
    }

    /// <summary>
    /// Updates the last scores of the players integrating its current score.
    /// </summary>
    /// <param name="scores">Player's previous last scores</param>
    void UpdateScores(ref int[] scores)
    {
        int previousScore = (int)Mathf.Floor(playerScore);
        int[] newScores = new int[scores.Length + 1];

        for (int i = 0; i < scores.Length; i++)
        {
            int tmp = scores[i];
            newScores[i] = previousScore;
            previousScore = tmp;
        }
        newScores[newScores.Length - 1] = previousScore;

        if (newScores.Length > 5)
        {
            Array.Resize(ref newScores, 5);
        }

        scores = newScores;
        SetLastScores(scores);
    }

    /// <summary>
    /// Saves the player's last scores in the PlayerPrefs.
    /// </summary>
    /// <param name="scores">Player's last scores.</param>
    void SetLastScores(int[] scores)
    {
        string serializedScores = "";
        for (int i = 0; i < scores.Length; i++)
        {
            serializedScores += scores[i] + ",";
        }
        serializedScores = serializedScores.Remove(serializedScores.Length - 1);

        PlayerPrefs.SetString("last_scores", serializedScores);
    }

    /// <summary>
    /// Returns the average of the provided int[].
    /// </summary>
    /// <param name="scores">Scores.</param>
    /// <returns>The average of the provided int[].</returns>
    float GetAverage(int[] scores)
    {
        int sum = 0;
        foreach (int score in scores)
        {
            sum += score;
        }

        return sum / scores.Length;
    }

    /// <summary>
    /// Hides all the blocks.
    /// </summary>
    void HideAll()
    {
        defaultBlock.SetActive(false);
        continueBlock.SetActive(false);

        newRecordBlock.SetActive(false);
    }
}
