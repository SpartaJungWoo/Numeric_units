using UnityEngine;

/// <summary>
/// Spawns an IndependantSound in the scene.
/// </summary>
public class SoundSpawner : MonoBehaviour
{
    /// <summary>
    /// Reference to the sound to play
    /// </summary>
    public IndependantSound soundToSpawn;

    /// <summary>
    /// Spawn the sound.
    /// </summary>
    public void Play()
    {
        Instantiate(soundToSpawn);
    }
}
