using UnityEngine;

/// <summary>
/// TODO: add comment
/// </summary>
public class MovingSphere : MonoBehaviour
{
    /// <summmary>
    /// TODO: add comment
    /// </summary>
    [SerializeField, Range(0f, 100f)]
    private float maxSpeed = 10f;

    /// <summmary>
    /// TODO: add comment
    /// </summary>
    [SerializeField, Range(0f, 100f)]
    private float maxAcceleration = 10f;

    /// <summmary>
    /// TODO: add comment
    /// </summary>
    [SerializeField]
    private Rect allowedArea = new Rect(-5f, -5f, 10f, 10f);

    /// <summmary>
    /// TODO: add comment
    /// </summary>
    [SerializeField, Range(0f, 1f)]
    private float bounciness = 0.5f;

    /// <summary>
    /// TODO: add comment
    /// </summary>
    private Vector3 velocity;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
        Vector3 displacement = velocity * Time.deltaTime;
        Vector3 newPosition = transform.localPosition + displacement;

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
