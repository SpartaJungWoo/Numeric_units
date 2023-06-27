using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject to define the configuration of the game.
/// </summary>
[CreateAssetMenu(fileName = "DifficultyConfigAsset", menuName = "Dash Runner/Config/DifficultyConfigAsset")]
[System.Serializable]
public class DifficultyConfigAsset : ScriptableObject
{
    /// <summary>
    /// Distance the player has to reach to play at this difficulty level.
    /// </summary>
    public float distanceToReachToUnlockThisDifficulty;

    /// <summary>
    /// Coefficient to apply to the general game speed.
    /// </summary>
    public float speedCoefficient;

    /// <summary>
    /// Obstacles that can spawn at the level of difficulty.
    /// </summary>
    public List<ObstacleGroup> obstacles;
}
