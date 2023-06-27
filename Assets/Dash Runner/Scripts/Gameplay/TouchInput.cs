using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TouchInput : MonoBehaviour
{
    [System.Serializable]
    public class TouchSwipeEvent : UnityEvent<Vector2> { }

    public float maxSwipeTime = 0.3f;
    [Range(0, 1)]
    public float minDistancePercentageOnScreen = 0.1f;

    public TouchSwipeEvent Swipe;

    Touch touch;
    Vector2 touchStartPosition;
    float touchStartTime;

    float minDelta;

    void Awake()
    {
        minDelta = minDistancePercentageOnScreen * Screen.height;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPosition = touch.position;
                touchStartTime = Time.time;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Vector2 delta = touch.position - touchStartPosition;

                if (Mathf.Abs(delta.y) > minDelta && (Time.time - touchStartTime) <= maxSwipeTime)
                {
                    Swipe.Invoke(delta.normalized);
                }
            }
        }
    }
}
