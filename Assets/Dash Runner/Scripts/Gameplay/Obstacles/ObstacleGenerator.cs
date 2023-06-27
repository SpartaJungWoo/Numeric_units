using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    /// <summary>
    /// Obstacles to use.
    /// </summary>
    List<ObstacleGroup> obstacles;
    public List<ObstacleGroup> Obstacles
    {
        get
        {
            return obstacles;
        }

        set
        {
            obstacles = value;
        }
    }

    /// <summary>
    /// Reference to the last generated obstacle, to set the good position for the next one.
    /// </summary>
    public Vector3 LastGeneratedObstacle
    {
        get
        {
            return generatedObstacles.Last<ObstacleGroup>().transform.position;
        }
    }

    /// <summary>
    /// List of the generated obstacles.
    /// </summary>
    List<ObstacleGroup> generatedObstacles;

    void Awake()
    {
        generatedObstacles = new List<ObstacleGroup>();
    }

    void Update()
    {
        if (generatedObstacles.Count > 0)
        {
            if (LastGeneratedObstacle.x < transform.position.x)
            {
                GenerateObstacle();
            }
        }
    }

    /// <summary>
    /// Initializes the obstacles.
    /// </summary>
    public void Init(List<ObstacleGroup> obstacles)
    {
        Obstacles = obstacles;

        GenerateObstacle();
    }

    /// <summary>
    /// Generates a random obstacle after the current one.
    /// </summary>
    public void Generate()
    {
        GenerateObstacle();
    }

    /// <summary>
    /// Generates the specified obstacle after the current one.
    /// </summary>
    /// <param name="obstacleGroup">The obstacle to generate.</param>
    public void Generate(ObstacleGroup obstacleGroup)
    {
        GenerateObstacle(obstacleGroup);
    }

    /// <summary>
    /// Delete all the obstacles on the scene and create a new one.
    /// </summary>
    public void Continue()
    {
        foreach (ObstacleGroup obstacle in generatedObstacles)
        {
            Destroy(obstacle.gameObject);
        }
        generatedObstacles.Clear();

        Generate();
    }

    /// <summary>
    /// Generates a random obstacle after the current one.
    /// </summary>
    void GenerateObstacle()
    {
        if (obstacles != null)
        {
            ObstacleGroup obstacleToInstantiate = obstacles[Random.Range(0, obstacles.Count)];
            GenerateObstacle(obstacleToInstantiate);
        }
    }

    /// <summary>
    /// Generates the specified obstacle after the current one.
    /// </summary>
    /// <param name="obstacleToInstantiate">Obstacle to generate.</param>
    void GenerateObstacle(ObstacleGroup obstacleToInstantiate)
    {
        float offset = obstacleToInstantiate.Size;
        if (generatedObstacles.Count > 0)
        {
            offset = generatedObstacles.Last().Size;
        }
        GenerateObstacle(obstacleToInstantiate, transform.position + Vector3.right * ((obstacleToInstantiate.Size + offset) / 2));
    }

    /// <summary>
    /// Generates the specified obstacle at the specified position.
    /// </summary>
    /// <param name="obstacleToInstantiate">Obstacle to generate.</param>
    /// <param name="position">Position of the obstacle.</param>
    void GenerateObstacle(ObstacleGroup obstacleToInstantiate, Vector2 position)
    {
        position.y = 0;
        ObstacleGroup instance = Instantiate<ObstacleGroup>(obstacleToInstantiate, position, Quaternion.identity);
        instance.Destroyed += OnObstacleDestroyed;
        generatedObstacles.Add(instance);
    }

    /// <summary>
    /// Obstacle destroyed handler.
    /// </summary>
    /// <param name="sender">Destroyed obstacle group.</param>
    void OnObstacleDestroyed(ObstacleGroup sender)
    {
        if (generatedObstacles.Contains(sender))
        {
            generatedObstacles.Remove(sender);
        }
    }
}
