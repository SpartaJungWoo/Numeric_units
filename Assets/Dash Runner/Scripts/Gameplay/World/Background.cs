using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    /// <summary>
    /// Speed to translate the background to give a paralax effect.
    /// </summary>
    public float counterSlidingSpeed = 0;

    /// <summary>
    /// Reference to the other background.
    /// </summary>
    public Transform otherBackground;

    /// <summary>
    /// Reference to the BoxCollider2D component.
    /// </summary>
    BoxCollider2D boxCollider;

    /// <summary>
    /// Reference to the Hero component.
    /// </summary>
    Hero hero;

    /// <summary>
    /// BoxCollider size by the scale.
    /// </summary>
    float realSize;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        realSize = boxCollider.size.x * transform.localScale.x;
    }

    void Start()
    {
        hero = MainGameManager.Instance.hero;
    }

    void Update()
    {
        if (hero != null)
        {
            if (hero.IsAlive)
            {
                transform.Translate(Vector3.left * counterSlidingSpeed * hero.SpeedCoefficient * Time.deltaTime);
            }
        }

        //Vector3 centerOfTheScreen = GetScreenCenterWorldPosition();
        if (transform.localPosition.x < -realSize)
        {
            transform.position = otherBackground.transform.position + transform.right * realSize;
        }
    }

    /// <summary>
    /// Calculate the world position of the center of the screen.
    /// </summary>
    /// <returns>The world position of the center of the screen.</returns>
    Vector3 GetScreenCenterWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
    }
}
