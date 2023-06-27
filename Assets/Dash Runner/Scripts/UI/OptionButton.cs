using UnityEngine;

public class OptionButton : MonoBehaviour
{
    /// <summary>
    /// Name of the key in the player prefs to change.
    /// </summary>
    public string optionToToggle;

    /// <summary>
    /// Default value for this option.
    /// </summary>
    public bool defaultValue;

    /// <summary>
    /// Reference to the ANimator component.
    /// </summary>
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        int optionValue = PlayerPrefs.GetInt(optionToToggle, defaultValue ? 1 : 0);
        if (optionValue == 1)
        {
            animator.Play("Activated");
        }
        else
        {
            animator.Play("Deactivated");
        }
        PlayerPrefs.SetInt(optionToToggle, optionValue);
    }

    /// <summary>
    /// Change the option value.
    /// </summary>
    public void Toggle()
    {
        int optionValue = PlayerPrefs.GetInt(optionToToggle, defaultValue ? 1 : 0);
        optionValue = optionValue == 0 ? 1 : 0;
        animator.SetTrigger("toggle");
        PlayerPrefs.SetInt(optionToToggle, optionValue);
    }
}
