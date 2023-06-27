using UnityEngine;
using UnityEngine.Audio;

public class SoundMuter : MonoBehaviour
{
    /// <summary>
    /// Reference to the Snapshot with all sounds activated.
    /// </summary>
    public AudioMixerSnapshot normalState;

    /// <summary>
    /// Reference to the Snapshot with all sounds muted.
    /// </summary>
    public AudioMixerSnapshot muteState;
    
    void Update()
    {
        int optionValue = PlayerPrefs.GetInt("sound", 1);
        switch (optionValue)
        {
            case 0:
                Mute();
                break;

            case 1:
                Unmute();
                break;
        }
    }

    /// <summary>
    /// Mutes sounds.
    /// </summary>
    public void Mute()
    {
        muteState.TransitionTo(0);
    }

    /// <summary>
    /// Unmutes sounds.
    /// </summary>
    public void Unmute()
    {
        normalState.TransitionTo(0);
    }
}
