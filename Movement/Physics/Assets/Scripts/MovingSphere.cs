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
    /// Instance variable <c>maxAirAcceleration</c> represents the maximum acceleration value of the sphere while in the air.
    /// </summary>
    [SerializeField, Range(0f, 100f)]
    private float maxAirAcceleration = 1f;

    /// <summmary>
    /// Instance variable <c>jumpHeight</c> represents the height value to expected to reach on player character's jump.
    /// </summary>
    [SerializeField, Range(0f, 10f)]
    private float jumpHeight = 2f;

    /// <summmary>
    /// Instance variable <c>maxAirJumps</c> represents the maximum value of air jumps allowed to the sphere.
    /// </summary>
    [SerializeField, Range(0, 5)]
    private int maxAirJumps = 0;

    /// <summmary>
    /// Instance variable <c>maxGroundAngle</c> represents the maximum angle value of a surface to be considered as a ground.
    /// </summary>
    [SerializeField, Range(0f, 90f)]
    private float maxGroundAngle = 25f;

    /// <summary>
    /// Instance variable <c>body</c> is a Unity <c>Rigidbody</c> component representing the rigidbody manager of the sphere.
    /// </summary>
    private Rigidbody body;

    /// <summary>
    /// Instance variable <c>velocity</c> is a Unity <c>Vector3</c> structure representing the velocity vector of the sphere.
    /// </summary>
    private Vector3 velocity;

    /// <summary>
    /// Instance variable <c>desiredVelocity</c> is a Unity <c>Vector3</c> structure representing the targeted velocity vector of the sphere.
    /// </summary>
    private Vector3 desiredVelocity;

    /// <summary>
    /// Instance variable <c>desiredJump</c> represents the player desiring jump status of the sphere.
    /// </summary>
    private bool desiredJump;

    /// <summary>
    /// Instance variable <c>onGround</c> represents on ground status of the sphere.
    /// </summary>
    private bool OnGround => groundContactCount > 0;

    /// <summary>
    /// Instance variable <c>groundContactCount</c> represents the current number of ground contact of the sphere.
    /// </summary>
    private int groundContactCount;

    /// <summary>
    /// Instance variable <c>jumpPhase</c> represents the number of sphere current successive jumps.
    /// </summary>
    private int jumpPhase;

    /// <summary>
    /// TODO: add comment
    /// </summary>
    private float minGroundDotProduct;

    /// <summary>
    /// Instance variable <c>contactNormal</c> is a Unity <c>Vector3</c> structure representing the current ground contact normal of the sphere.
    /// </summary>
    private Vector3 contactNormal;

    #endregion

    #region MonoBehavior

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        OnValidate();
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

        // Check for jump input in "Update", delay jump to the next "FixedUpdate" frame for physics consideration.
        // OR assignement to prevent jump not being called on input because of eventual "FixedUpdate" not invoked next frame.
        desiredJump |= Input.GetButtonDown("Jump");

        GetComponent<Renderer>().material.SetColor("_Color", Color.white * (groundContactCount * 0.25f));
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void FixedUpdate()
    {
        UpdateState();
        AdjustVelocity();

        if (desiredJump)
        {
            desiredJump = false;
            Jump();
        }
        body.velocity = velocity;
        ClearState();
    }

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun
    /// touching another rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    private void OnCollisionEnter(Collision other)
    {
        EvaluateCollision(other);
    }

    /// <summary>
    /// OnCollisionStay is called once per frame for every collider/rigidbody
    /// that is touching rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    private void OnCollisionStay(Collision other)
    {
        EvaluateCollision(other);
    }

    /// <summary>
    /// Called when the script is loaded or a value is changed in the
    /// inspector (Called in the editor only).
    /// </summary>
    void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    #endregion

    #region Private

    /// <summary>
    /// This function is called when we need the character to jump.
    /// </summary>
    private void Jump()
    {
        // Allow jumping only on ground or in max number of air jumps isn't exceeded.
        if (OnGround || jumpPhase < maxAirJumps)
        {
            jumpPhase++;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            // Project the velocity on the ocntact normal to check for positive aligned speed value.
            float alignedSpeed = Vector3.Dot(velocity, contactNormal);
            // To limit upward velocity when jumping successively.
            if (alignedSpeed > 0f)
            {
                // Use "Mathf.Max" to not slowing down the sphere when jumping with already a high upward velocity.
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            }
            velocity += contactNormal * jumpSpeed;
        }
    }

    /// <summary>
    /// This function is responsible for evaluating the sphere collisions with other objects.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    private void EvaluateCollision(Collision other)
    {
        // If the y normal of object colliding with the sphere isn't 0.9 or greater then we will consider it's not a ground.
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.GetContact(i).normal;
            if (normal.y >= minGroundDotProduct)
            {
                groundContactCount++;
                // Sum normals to represents an average ground plane.
                contactNormal += normal;
            }
        }
    }

    /// <summary>
    /// This function is responsible updating the movement state of the sphere.
    /// </summary>
    private void UpdateState()
    {
        velocity = body.velocity;
        if (OnGround)
        {
            jumpPhase = 0;
            if (groundContactCount > 1)
            {
                contactNormal.Normalize();
            }
        }
        else
        {
            contactNormal = Vector3.up;
        }
    }

    /// <summary>
    /// This function is responsible for computing a projected vector of the given one, over the current contact plane of the sphere.
    /// </summary>
    /// <param name="vector">A Unity <c>Vector3</c> structure representing the vector to be projected.</param>
    /// <returns>A Unity <c>Vector3</c> structure representing the given vector with values projected over the current contact plane of the sphere.</returns>
    private Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

    /// <summary>
    /// This function is responsible for adjusting and computing the velocity regarding the different situation the sphere can face (slope, flat ground, multiple ground contacts, etc.).
    /// </summary>
    private void AdjustVelocity()
    {
        // Compute vectors aligned with the sphere ground (only a unit length).
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

        // Project current velocity on both vectors aligned with the sphere ground to compute the relative X and Z speeds.
        float currentX = Vector3.Dot(velocity, xAxis);
        float currentZ = Vector3.Dot(velocity, zAxis);

        // Compute maximum speed change depending player max acceleration (change dependently of sphere on ground or in air).
        float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        // Compute the new X and Z speeds (taking account current acceleration) but now relative to the current ground surface.
        float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

        // Adjust the velocity by adding the differences between the new and current speeds along the relatives axes.
        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }

    /// <summary>
    /// This function is responsible clearing the movement state of the sphere.
    /// </summary>
    private void ClearState()
    {
        groundContactCount = 0;
        contactNormal = Vector3.zero;
    }

    #endregion
}