using UnityEngine;

public class HeroFollower : MonoBehaviour
{
    /// <summary>
    /// Reference to the Hero's Transform to follow.
    /// </summary>
    public Transform heroTransform;

    /// <summary>
    /// Smooth damp value.
    /// </summary>
    public float smoothDampValue;

    /// <summary>
    /// Value used for the SmoothDamp function.
    /// </summary>
    Vector3 velocity;

    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, heroTransform.position, ref velocity, smoothDampValue);
    }
}
