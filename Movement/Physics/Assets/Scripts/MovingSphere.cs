using UnityEngine;

/// <summary>
/// Class <c>MovingSphere</c> is a Unity component script used to manage the sphere movement behavior.
/// </summary>
public class MovingSphere : MonoBehaviour
{
    #region Fields / Properties

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
    /// Instance variable <c>velocity</c> is a Unity <c>Vector3</c> structure representing the velocity vector of the sphere.
    /// </summary>
    private Vector3 velocity;

    /// <summary>
    /// Instance variable <c>desiredVelocity</c> is a Unity <c>Vector3</c> structure representing the targeted velocity vector of the sphere.
    /// </summary>
    private Vector3 desiredVelocity;

    /// <summary>
    /// Instance variable <c>body</c> is a Unity <c>Rigidbody</c> component representing the rigidbody manager of the sphere.
    /// </summary>
    private Rigidbody body;

    #endregion

    #region MonoBehavior

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

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
        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void FixedUpdate()
    {
        // Retrieve velocity from rigidbody before updating it. (Physics collisions and such affect velocity)
        velocity = body.velocity;
        // Compute maximum speed change depending player max acceleration.
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        body.velocity = velocity;
    }

    #endregion
}