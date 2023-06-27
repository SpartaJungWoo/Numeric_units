using UnityEngine;

/// <summary>
/// Class to be able to play a cross-scene sounds and to destroy it automatically at the end of it.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class IndependantSound : MonoBehaviour
{
    /// <summary>
    /// Reference to the AudioSource.
    /// </summary>
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        if (!audioSource.playOnAwake)
        {
            audioSource.Play();
        }
    }

    void Update()
    {
        if (audioSource.time / audioSource.clip.length >= 0.95f)
        {
            Destroy(gameObject);
        }
    }
}