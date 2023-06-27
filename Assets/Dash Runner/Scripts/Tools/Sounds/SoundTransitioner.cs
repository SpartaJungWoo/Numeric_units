using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundTransitioner : MonoBehaviour
{
    /// <summary>
    /// Reference to the Snapshot with all sounds activated.
    /// </summary>
    public AudioMixerSnapshot normalState;

    /// <summary>
    /// Reference to the Snapshot with all sounds muted.
    /// </summary>
    public AudioMixerSnapshot muteMusicState;

    public void FadeIn()
    {
        muteMusicState.TransitionTo(0);
        StartCoroutine(WaitFrameForTransition(normalState));
    }

    public void FadeOut()
    {
        normalState.TransitionTo(0);
        StartCoroutine(WaitFrameForTransition(muteMusicState));
    }

    IEnumerator WaitFrameForTransition(AudioMixerSnapshot snapshotToTransitionTo)
    {
        yield return null;

        snapshotToTransitionTo.TransitionTo(1);
    }
}
