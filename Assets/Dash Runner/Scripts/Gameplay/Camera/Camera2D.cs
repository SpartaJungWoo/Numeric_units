using System.Collections.Generic;
using UnityEngine;

public class Camera2D : MonoBehaviour
{
    /// <summary>
    /// Targets to follow. If multiple targets, then place the camera at the barycentre of the targets.
    /// </summary>
    public List<Transform> targets;

    /// <summary>
    /// Minimum limit on X axis.
    /// </summary>
    [SerializeField]
    public float minX = -10.0f;
    /// <summary>
    /// Maximum limit on X axis.
    /// </summary>
    [SerializeField]
    public float maxX = 10.0f;
    /// <summary>
    /// Minimum limit on Y axis.
    /// </summary>
    [SerializeField]
    public float minY = -10.0f;
    /// <summary>
    /// Maximum limit on Y axis.
    /// </summary>
    [SerializeField]
    public float maxY = 10.0f;
    /// <summary>
    /// Minimum limit on Z axis.
    /// </summary>
    [SerializeField]
    public float minZ = -10.0f;
    /// <summary>
    /// Maximum limit on Z axis.
    /// </summary>
    [SerializeField]
    public float maxZ = 10.0f;

    /// <summary>
    /// Offset to apply to the calculated camera's destination.
    /// </summary>
    public Vector3 offset;

    /// <summary>
    /// Minimum camera size.
    /// </summary>
    public float minCameraSize = 5;

    /// <summary>
    /// Maximum camera size.
    /// </summary>
    public float maxCameraSize = 10;

    /// <summary>
    /// Padding to add to the camera size.
    /// </summary>
    public float cameraSizePadding = 5;

    /// <summary>
    /// Lag in second for the camera to follow the target.
    /// </summary>
    public float followLag = 0.5f;

    /// <summary>
    /// Speed with which the camera changes its size.
    /// </summary>
    public float zoomSpeed = 10;

    /// <summary>
    /// Reference to the camera script.
    /// </summary>
    new Camera camera;

    /// <summary>
    /// Target size to reach on next frames.
    /// </summary>
    float targetSize;

    /// <summary>
    /// Used for camera movements.
    /// </summary>
    Vector3 velocity = Vector3.zero;

    void Awake()
    {
        camera = GetComponentInChildren<Camera>();
    }

    void LateUpdate()
    {
        if (CanPerform())
        {
            Perform();
        }
    }

    /// <summary>
    /// Adds a new target to follow.
    /// </summary>
    /// <param name="target"></param>
    public void AddTarget(Transform target)
    {
        targets.Add(target);
    }

    /// <summary>
    /// Removes a target to follow.
    /// </summary>
    /// <param name="target">The target to remove.</param>
    public void RemoveTarget(Transform target)
    {
        if (targets.Contains(target))
        {
            targets.Remove(target);
        }
    }

    /// <summary>
    /// Can the script perform normally?
    /// </summary>
    /// <returns></returns>
    bool CanPerform()
    {
        return targets != null && targets.Count > 0;
    }

    /// <summary>
    /// Performs this script behaviour.
    /// </summary>
    void Perform()
    {
        MoveToward(Clamp(ApplyOffset(GetPositionToReach())));

        if (targets.Count > 1)
        {
            CalculateTargetSize();
            ApplyZoom();
        }
    }

    /// <summary>
    /// Calculates the targeted camera size depending on the position of all the targets.
    /// </summary>
    void CalculateTargetSize()
    {
        // Gets the boundaries
        float minTargetX = float.MaxValue;
        float maxTargetX = float.MinValue;
        float minTargetY = float.MaxValue;
        float maxTargetY = float.MinValue;

        float cameraEulerXAngle = camera.transform.rotation.eulerAngles.x;
        float yCoefficient = Mathf.Cos(Mathf.Deg2Rad * cameraEulerXAngle);
        float zCoefficient = Mathf.Sin(Mathf.Deg2Rad * cameraEulerXAngle);

        foreach (Transform target in targets)
        {
            if (target.position.x < minTargetX)
            {
                minTargetX = target.position.x;
            }
            if (target.position.x > maxTargetX)
            {
                maxTargetX = target.position.x;
            }

            // Applies a coefficient to y and z coordinates to fit the rotation of the camera.
            float yPosition = target.position.y * yCoefficient + target.position.z * zCoefficient;

            if (yPosition < minTargetY)
            {
                minTargetY = yPosition;
            }
            if (yPosition > maxTargetY)
            {
                maxTargetY = yPosition;
            }
        }

        // Adds padding to the boundaries.
        minTargetX -= cameraSizePadding;
        minTargetY -= cameraSizePadding;
        maxTargetX += cameraSizePadding;
        maxTargetY += cameraSizePadding;

        // Calculates the required size of the camera.
        float requiredWidth = maxTargetX - minTargetX;
        float requiredHeight = maxTargetY - minTargetY;
        float maxWidth = maxX - minX;
        float maxHeight = maxY - minY;

        if (requiredWidth > maxWidth)
        {
            requiredWidth = maxWidth;
        }
        if (requiredHeight > maxHeight)
        {
            requiredHeight = maxHeight;
        }

        float sizeToFitHeight = requiredHeight / 2;
        float sizeToFitWidth = requiredWidth / (2 * camera.aspect);

        targetSize = Mathf.Max(sizeToFitHeight, sizeToFitWidth);

        float futurHeight = targetSize * 2;
        float futurWitdh = futurHeight * camera.aspect;

        if (futurHeight > maxHeight)
        {
            targetSize = 2 * maxHeight;
        }

        if (futurWitdh > maxWidth)
        {
            targetSize = maxWidth / (2 * camera.aspect);
        }

        targetSize = Mathf.Clamp(targetSize, minCameraSize, maxCameraSize);
    }

    /// <summary>
    /// Get the position to reach by calculating the isobarycentre of all the targets.
    /// </summary>
    /// <returns>The isobarycentre of all the targets.</returns>
    Vector3 GetPositionToReach()
    {
        Vector3 position = Vector3.zero;

        foreach (Transform target in targets)
        {
            position += target.position;
        }

        position /= targets.Count;

        return position;
    }

    /// <summary>
    /// Applies an offset to the provided Vector3.
    /// </summary>
    /// <param name="source">Vector3 to apply an offset on.</param>
    /// <returns>The offset Vector3.</returns>
    Vector3 ApplyOffset(Vector3 source)
    {
        source += offset;

        return source;
    }

    /// <summary>
    /// Clamp the provided Vector3 within the limits.
    /// </summary>
    /// <param name="source">Vector3 to clamp.</param>
    /// <returns>The clamped Vector3.</returns>
    Vector3 Clamp(Vector3 source)
    {
        float verticalSize = camera.orthographicSize * 2.0f;
        float horizontalSize = verticalSize * camera.aspect;

        float cameraEulerXAngle = camera.transform.rotation.eulerAngles.x;
        float cosinus = Mathf.Cos(Mathf.Deg2Rad * cameraEulerXAngle);
        float sinus = Mathf.Sin(Mathf.Deg2Rad * cameraEulerXAngle);
        float calculatedMaxY = maxY - sinus * camera.nearClipPlane - cosinus * camera.orthographicSize;
        float calculatedMinY = minY + sinus * camera.nearClipPlane + cosinus * camera.orthographicSize;
        float calculatedMaxZ = maxZ - cosinus * camera.nearClipPlane - sinus * camera.orthographicSize;
        float calculatedMinZ = minZ + cosinus * camera.nearClipPlane + sinus * camera.orthographicSize;

        source.x = Mathf.Clamp(source.x, minX + horizontalSize / 2, maxX - horizontalSize / 2);
        source.y = Mathf.Clamp(source.y, calculatedMinY, calculatedMaxY);
        source.z = Mathf.Clamp(source.z, calculatedMinZ, calculatedMaxZ);

        return source;
    }
    /// <summary>
    /// Moves the camera toward the provided position.
    /// </summary>
    /// <param name="targetPosition">Position to move toward.</param>
    void MoveToward(Vector3 targetPosition)
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, followLag);
    }

    /// <summary>
    /// Applies the target size.
    /// </summary>
    void ApplyZoom()
    {
        camera.orthographicSize += (targetSize - camera.orthographicSize) / zoomSpeed;
    }
}
