using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Delegate for ObstacleGroup events.
/// </summary>
/// <param name="sender"></param>
public delegate void ObstacleGroupVoid(ObstacleGroup sender);

public class ObstacleGroup : MonoBehaviour
{
    /// <summary>
    /// Event triggered when the obstacle is destroyed.
    /// </summary>
    public ObstacleGroupVoid Destroyed;

    /// <summary>
    /// Reference to the BoxCollider2D component.
    /// </summary>
    BoxCollider2D boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (MainGameManager.Instance.hero.transform.position.x - transform.position.x > boxCollider.size.x)
        {
            Destroy();
        }
    }

    /// <summary>
    /// Accessor to the size of the group.
    /// </summary>
    public float Size
    {
        get
        {
            return GetComponent<BoxCollider2D>().size.x;
        }
    }

    /// <summary>
    /// Destroys this GameObject and request the ObstacleGenerator to create a new ObstacleGroup.
    /// </summary>
    public void Destroy()
    {
        if (Destroyed != null)
        {
            Destroyed(this);
        }

        Destroy(gameObject);
    }
}
