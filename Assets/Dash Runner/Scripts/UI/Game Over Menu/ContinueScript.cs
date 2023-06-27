using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContinueScript : MonoBehaviour
{
    /// <summary>
    /// Reference  to the GameOverMenu.
    /// </summary>
    public GameOverMenu gameOverMenu;

    /// <summary>
    /// Reference to the countdown circle.
    /// </summary>
    public Image countdownCircle;

    /// <summary>
    /// Reference to the Text displaying the countdown.
    /// </summary>
    public TextMeshProUGUI countdownText;

    /// <summary>
    /// Seconds left before the end of the countdown.
    /// </summary>
    float countdown;

    /// <summary>
    /// Is countdown on?
    /// </summary>
    bool isAlive = false;

    void OnEnable()
    {
        StartCountdown();
    }

    void OnDisable()
    {
        isAlive = false;
    }

    void Update()
    {
        if (isAlive)
        {
            countdown -= Time.deltaTime;
            countdownText.text = Mathf.Floor(countdown).ToString();
            countdownCircle.fillAmount = (float)(countdown / 4.0f);

            if (countdown < 0)
            {
                gameOverMenu.SwitchToDefault();
            }
        }
    }

    /// <summary>
    /// Stops the countdown.
    /// </summary>
    public void StopCountdown()
    {
        isAlive = false;
    }

    /// <summary>
    /// Launches the countdown.
    /// </summary>
    void StartCountdown()
    {
        countdown = 4;
        isAlive = true;
    }
}
