using UnityEngine;

/// <summary>
/// Class <c>MovingSphere</c> is a Unity component script used to manage the sphere movement behavior.
/// </summary>
public class MovingSphere : MonoBehaviour
{
    /// <summary>
    /// Instance variable <c>maxSpeed</c> represents the maximum speed value of the sphere.
    /// </summary>
    [SerializeField, Range(0f, 100f)]
    private float maxSpeed = 10f;

    /// <summary>
    /// Instance variable <c>maxAcceleration</c> represents the maximum acceleration value of the sphere.
    /// </summary>
    [SerializeField, Range(0f, 100f)]
    private float maxAcceleration = 10f;

    /// <summary>
    /// Instance variable <c>allowedArea</c> is a Unity <c>Rect</c> structure representing the rectangle dimensions definition of the sphere allowed area.
    /// </summary>
    [SerializeField]
    private Rect allowedArea = new Rect(-5f, -5f, 10f, 10f);

    /// <summary>
    /// Instance variable <c>bounciness</c> represents the bounciness value of the sphere.
    /// </summary>
    [SerializeField, Range(0f, 1f)]
    private float bounciness = 0.5f;

    /// <summary>
    /// Instance variable <c>velocity</c> is a Unity <c>Vector3</c> structure representing the velocity vector of the sphere.
    /// </summary>
    private Vector3 velocity;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        // Get player input value.
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");

        // Use "Normalize" for all-or-nothing input.
        // "ClampMagnitude" for a playerInput vector that is either the same or scaled down to the provided maximum.
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        // Compute desired velocity depending player inputs.
        Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        // Compute maximum speed change depending player max acceleration.
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        Vector3 displacement = velocity * Time.deltaTime;
        Vector3 newPosition = transform.localPosition + displacement;

        // Clamp the sphere inside the area and apply a little bounciness effect on sphere colliding with the area limits.
        if (newPosition.x < allowedArea.xMin)
        {
            newPosition.x = allowedArea.xMin;
            velocity.x = -velocity.x * bounciness;
        }
        else if (newPosition.x > allowedArea.xMax)
        {
            newPosition.x = allowedArea.xMax;
            velocity.x = -velocity.x * bounciness;
        }

        if (newPosition.z < allowedArea.yMin)
        {
            newPosition.z = allowedArea.yMin;
            velocity.z = -velocity.z * bounciness;
        }
        else if (newPosition.z > allowedArea.yMax)
        {
            newPosition.z = allowedArea.yMax;
            velocity.z = -velocity.z * bounciness;
        }

        transform.localPosition = newPosition;
    }
}
