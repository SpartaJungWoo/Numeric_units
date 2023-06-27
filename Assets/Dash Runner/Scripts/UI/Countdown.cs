using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class handling a generic countdown and throwing an event of over.
/// </summary>
public class Countdown : MonoBehaviour
{
    /// <summary>
    /// Time in seconds to wait for each item.
    /// </summary>
    [Range(0.5f, 2)]
    public float timeBetweenItems = 1;

    /// <summary>
    /// List of items to display, in the order of apparition.
    /// </summary>
    public GameObject[] items;

    /// <summary>
    /// Event dispatched when the countdown is over.
    /// </summary>
    public UnityEvent OnCountdownOver;

    /// <summary>
    /// Is the countdown on?
    /// </summary>
    bool activated;

    /// <summary>
    /// Reference to the current displayed item.
    /// </summary>
    GameObject currentItem;

    /// <summary>
    /// Index of the current displayed item.
    /// </summary>
    int currentItemIndex;

    /// <summary>
    /// Time when the current item has been displayed.
    /// </summary>
    float currentItemTime;

    void Update()
    {
        if (activated)
        {
            if (Time.time - currentItemTime > timeBetweenItems)
            {
                Destroy(currentItem);
                currentItemIndex++;

                if (currentItemIndex >= items.Length)
                {
                    activated = false;

                    if (OnCountdownOver != null)
                    {
                        OnCountdownOver.Invoke();
                    }
                }
                else
                {
                    currentItem = Instantiate<GameObject>(items[currentItemIndex], transform);
                    currentItemTime = Time.time;
                }
            }
        }
    }

    /// <summary>
    /// Launches the countdown.
    /// </summary>
    public void Launch()
    {
        currentItemIndex = -1;
        currentItemTime = -timeBetweenItems;
        activated = true;
    }

    /// <summary>
    /// Gets the full countdown length.
    /// </summary>
    /// <returns>Countdown length in seconds.</returns>
    public float CountdownLength()
    {
        return timeBetweenItems * items.Length;
    }
}
