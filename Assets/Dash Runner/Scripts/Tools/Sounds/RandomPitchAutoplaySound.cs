using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomPitchAutoplaySound : MonoBehaviour
{
    /// <summary>
    /// Minimum pitch for random
    /// </summary>
    [Range(-3, 3)]
    public float minPitch;

    /// <summary>
    /// Maximum pitch for random
    /// </summary>
    [Range(-3, 3)]
    public float maxPitch;

    /// <summary>
    /// Reference to the AudioSource component.
    /// </summary>
    new AudioSource audio;

    void Awake()
    {
        if (minPitch > maxPitch)
        {
            Debug.LogError("Min pitch is higher than max pitch on " + name);
            Destroy(gameObject);
        }

        audio = GetComponent<AudioSource>();
        audio.pitch = Random.Range(minPitch, maxPitch);
        audio.playOnAwake = true;
    }
}
