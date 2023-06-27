using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    /// <summary>
    /// List of the different difficulty levels.
    /// </summary>
    public List<DifficultyConfigAsset> difficultyConfigAssets;

    /// <summary>
    /// Assets sorted by distance to reach to unlock them.
    /// </summary>
    Dictionary<float, DifficultyConfigAsset> sortedDifficultyConfigAssets;

    /// <summary>
    /// Current difficulty level stored as the last distance reached to unlock the current DifficultConfigAsset.
    /// </summary>
    float currentDifficultyLevel;

    void Awake()
    {
        currentDifficultyLevel = -1;
        SortDifficultyConfigAssets();
    }

    /// <summary>
    /// Gets the next distance to reach to change the difficulty level.
    /// </summary>
    /// <returns>The next distance to reach to change the difficulty level.</returns>
    public float GetNextDifficultyStep()
    {
        float nextDistance = -1;
        foreach (float distance in sortedDifficultyConfigAssets.Keys)
        {
            if (distance > currentDifficultyLevel)
            {
                nextDistance = distance;
                break;
            }
        }

        return nextDistance;
    }

    /// <summary>
    /// Gets the DifficultyConfigAsset matching the given distance.
    /// </summary>
    /// <param name="distance">Key of the DifficultyConfigAsset to get. Can be get from the GetNextDifficultyStep function.</param>
    /// <returns>The matching DifficultyConfigAsset.</returns>
    public DifficultyConfigAsset GetDifficultyConfigAssetFor(float distance)
    {
        if (sortedDifficultyConfigAssets.ContainsKey(distance))
        {
            currentDifficultyLevel = distance;
            return sortedDifficultyConfigAssets[distance];
        }
        else
        {
            throw(new Exception("The given distance (" + distance + ") doesn't match any Config"));
        }
    }

    /// <summary>
    /// Fills the sortedDifficultyConfigAssets Dictionary.
    /// </summary>
    void SortDifficultyConfigAssets()
    {
        sortedDifficultyConfigAssets = new Dictionary<float, DifficultyConfigAsset>();

        // Gets all the distances steps.
        float[] distancesToReach = new float[difficultyConfigAssets.Count];
        int i = 0;
        foreach (DifficultyConfigAsset config in difficultyConfigAssets)
        {
            distancesToReach[i] = config.distanceToReachToUnlockThisDifficulty;
            i++;
        }

        // Sort them.
        Array.Sort(distancesToReach);

        // Fills the sortedDifficultyAssets Dictionary with sorted elements.
        foreach (float distance in distancesToReach)
        {
            foreach (DifficultyConfigAsset config in difficultyConfigAssets)
            {
                if (distance == config.distanceToReachToUnlockThisDifficulty)
                {
                    sortedDifficultyConfigAssets.Add(config.distanceToReachToUnlockThisDifficulty, config);
                }
            }
        }
    }
}
