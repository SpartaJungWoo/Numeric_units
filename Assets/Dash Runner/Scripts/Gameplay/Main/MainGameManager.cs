using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainGameManager : MonoBehaviour
{
    /// <summary>
    /// Reference to the Hero.
    /// </summary>
    public Hero hero;

    /// <summary>
    /// Reference to the ObstacleGenerator.
    /// </summary>
    public ObstacleGenerator obstacleGenerator;

    /// <summary>
    /// Reference to the distance text.
    /// </summary>
    public TextMeshProUGUI distanceText;

    /// <summary>
    /// Reference to the highscore text.
    /// </summary>
    public TextMeshProUGUI highscoreText;

    /// <summary>
    /// Reference to the in game UI Animator component.
    /// </summary>
    public Animator inGameUiAnimator;

    /// <summary>
    /// Reference to the countdown component.
    /// </summary>
    public Countdown countdown;

    /// <summary>
    /// Reference to the pause panel.
    /// </summary>
    public GameObject pausePanel;

    /// <summary>
    /// Reference to the game over panel.
    /// </summary>
    public GameObject gameOverPanel;

    /// <summary>
    /// List of buttons available during the game.
    /// </summary>
    public List<Button> interactableInGameButtons;

    /// <summary>
    /// Does the game launches itself?
    /// </summary>
    /// <returns></returns>
    public bool playOnAwake = false;

    /// <summary>
    /// Instance of the <see cref="MainGameManager"/>'s singleton.
    /// </summary>
    static MainGameManager instance;
    /// <summary>
    /// Gets the <see cref="MainGameManager"/>'s singleton.
    /// </summary>
    public static MainGameManager Instance
    {
        get
        {
            return instance;
        }
    }

    /// <summary>
    /// Reference to the DifficultyManager.
    /// </summary>
    DifficultyManager difficultyManager;

    /// <summary>
    /// Is game on?
    /// </summary>
    bool isAlive = false;

    /// <summary>
    /// Current distance in game.
    /// </summary>
    public float CurrentDistance { get; private set; }

    /// <summary>
    /// Distance at which the player starts the game.
    /// </summary>
    float startingDistance;

    /// <summary>
    /// Distance to reach in order to change the difficulty level.
    /// </summary>
    float nextDistanceStep;

    /// <summary>
    /// Current DifficultyConfigAsset to apply.
    /// </summary>
    DifficultyConfigAsset currentDifficultyConfig;

    /// <summary>
    /// Reference to the trail following the Hero.
    /// </summary>
    HeroFollower trail;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        difficultyManager = GetComponent<DifficultyManager>();
        trail = FindObjectOfType<HeroFollower>();

        PrepareScene();
    }

    void Start()
    {
        trail.gameObject.SetActive(false);

        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);

        ToggleInteractableButtons(false);

        PrepareGame();
    }

    void Update()
    {
        if (isAlive)
        {
            CurrentDistance = hero.transform.position.x - startingDistance;

            UpdateDifficulty();
            UpdateDistanceText();
        }
    }

    /// <summary>
    /// Prepares the scene.
    /// </summary>
    void PrepareScene()
    {
        CurrentDistance = 0;
        UpdateDistanceText();
        highscoreText.text = PlayerPrefs.GetFloat("record", 0).ToString("N00").Replace(',', ' ');
    }

    /// <summary>
    /// Prepares the game.
    /// </summary>
    void PrepareGame()
    {
        nextDistanceStep = difficultyManager.GetNextDifficultyStep();
        currentDifficultyConfig = difficultyManager.GetDifficultyConfigAssetFor(nextDistanceStep);

        inGameUiAnimator.SetTrigger("display");

        countdown.OnCountdownOver.AddListener(LaunchGame);
        countdown.Launch();
    }

    /// <summary>
    /// Launches the game.
    /// </summary>
    void LaunchGame()
    {
        countdown.OnCountdownOver.RemoveListener(LaunchGame);

        startingDistance = hero.transform.position.x;

        hero.StartGame();
        hero.SpeedCoefficient = currentDifficultyConfig.speedCoefficient;
        trail.gameObject.SetActive(true);

        obstacleGenerator.Init(currentDifficultyConfig.obstacles);

        ToggleInteractableButtons(true);

        isAlive = true;
        Time.timeScale = 1;
    }

    /// <summary>
    /// Deactivates the UI.
    /// </summary>
    public void DeactivateUI()
    {
        ToggleInteractableButtons(false, false);
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void PauseGame()
    {
        if (isAlive)
        {
            hero.Pause(true);
            isAlive = false;
            pausePanel.SetActive(true);

            Time.timeScale = 0;
            
            ToggleInteractableButtons(false);
        }
    }

    /// <summary>
    /// Resumes the game.
    /// </summary>
    public void ResumeGame()
    {
        hero.Pause(false);
        isAlive = true;
        pausePanel.SetActive(false);

        Time.timeScale = 1;
        
        ToggleInteractableButtons(true);
    }

    /// <summary>
    /// Prepare the GameOver screen.
    /// </summary>
    public void PreGameOver()
    {
        isAlive = false;
        trail.gameObject.SetActive(false);
        ToggleInteractableButtons(false);
    }

    /// <summary>
    /// Displays the game over panel.
    /// </summary>
    public void GameOver()
    {
        isAlive = false;
        ToggleInteractableButtons(false);

        float playerRecord = PlayerPrefs.GetFloat("record", 0);
        if (playerRecord < CurrentDistance)
        {
            PlayerPrefs.SetFloat("record", Mathf.Floor(CurrentDistance));
        }
        
        gameOverPanel.SetActive(true);
        gameOverPanel.GetComponent<GameOverMenu>().Init(CurrentDistance, playerRecord);
    }

    /// <summary>
    /// Resume the game as it is.
    /// </summary>
    public void Continue()
    {
        isAlive = true;
        gameOverPanel.SetActive(false);

        hero.PrepareToContinue();

        obstacleGenerator.Continue();

        ToggleInteractableButtons(true);

        countdown.OnCountdownOver.AddListener(GoContinue);
        countdown.Launch();
    }

    /// <summary>
    /// Launches the game.
    /// </summary>
    void GoContinue()
    {
        countdown.OnCountdownOver.RemoveListener(GoContinue);
        hero.Continue();
        trail.gameObject.SetActive(true);
    }

    /// <summary>
    /// Updates the difficulty
    /// </summary>
    void UpdateDifficulty()
    {
        if (CurrentDistance >= nextDistanceStep && nextDistanceStep != -1) // -1 means maximum difficulty
        {
            currentDifficultyConfig = difficultyManager.GetDifficultyConfigAssetFor(nextDistanceStep);
            nextDistanceStep = difficultyManager.GetNextDifficultyStep();

            hero.SpeedCoefficient = currentDifficultyConfig.speedCoefficient;
            obstacleGenerator.Obstacles = currentDifficultyConfig.obstacles;
        }
    }

    /// <summary>
    /// Updates the distance text
    /// </summary>
    void UpdateDistanceText()
    {
        distanceText.text = Mathf.Floor(CurrentDistance).ToString("N00").Replace(',', ' ');
    }

    /// <summary>
    /// Toggle the in game available buttons.
    /// </summary>
    /// <param name="enabled"></param>
    void ToggleInteractableButtons(bool enabled, bool activated = true)
    {
        foreach (Button button in interactableInGameButtons)
        {
            button.enabled = enabled;
        }
    }
}
