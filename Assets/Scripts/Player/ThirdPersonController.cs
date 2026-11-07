using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

/// <summary>
/// This class acts as the primary controller for third person POV.
/// Here we set fields like movement speed, look sensitivity, and check for ground collisions.
/// </summary>
public class ThirdPersonController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpHeight = 6.2f;
    [SerializeField] private float jumpCooldown = .1f;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float fallGravityMultiplier = 2f;
    [SerializeField] private float lowJumpGravityMultiplier = 2.5f;
    [SerializeField] private float dragForce = 3f;

    [Header("Grounded")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Cinemachine")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float topClampAngle = 60f;
    [SerializeField] private float bottomClampAngle = -30f;

    [Header("Sensitivity")]
    [SerializeField] private float lookSensitivity = 5f;

    [Header("Wall Handling")]
    [SerializeField] private float wallCheckDistance = 0.1f;
    [SerializeField] private float maxWalkableNormalY = 0.7f; // surfaces steeper than this count as walls

    [Header("Other")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float animatorRunningSpeed = 2.0f;
    [SerializeField] private float animatorWalkingSpeed = 1.0f;
    public Animator playerAnimator;

    [Header("Dashing")]
    [SerializeField] private float dashPower = 20;
    [SerializeField] private float dashUpwardPower = 0;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCooldown = 0.7f;
    [SerializeField] private float dashSpeedIncrease = 10; // how much the player move speed should increase by during the dash
    [SerializeField] private Transform orientation;
    private bool isDashHeld;
    private bool canDash = true;
    public bool allowAllDirections = true;
    public bool disableGravity = true;
    public bool resetVel = true;

    // Movement and Look Vectors //
    private Vector2 move;
    private Vector2 look;

    // Camera Variables //
    private float yaw;
    private float pitch;
    private const float lookThreshold = 0.01f;

    // Movement-Related Variables //
    private bool isGrounded = true;
    private bool canJump = true;
    private bool isRunning;
    private bool isJumpHeld;
    private float currentSpeed = 0f;
    private float acceleration = 3f;
    private bool MovementLocked = false;

    ////////////////////////////////// GENERIC METHODS //////////////////////////////////////

    /// <summary>
    /// Runs on start
    /// </summary>
    private void Start()
    {
        if (rb == null)
        {
            Debug.LogError("No Rigidbody found, disabling ThirdPersonController.");
            enabled = false;
            return;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Runs every frame (before LateUpdate)
    /// </summary>
    private void Update()
    {
        GroundedCheck();
    }

    /// <summary>
    /// Runs every frame (after Update)
    /// </summary>
    private void LateUpdate()
    {
        Look();
    }

    /// <summary>
    /// Runs at a fixed framerate of 50Hz
    /// </summary>
    /// <remarks>
    /// Use for phsyics-related functionality
    /// </remarks>
    private void FixedUpdate()
    {
        Move();
        ApplyJumpGravity();
    }

    /// <summary>
    /// Generic method for clamping an angle between a minimum and maximum
    /// </summary>
    /// <param name="angle"></param> the angle you want to clamp
    /// <param name="min"></param> the minimum value you want the angle to clamp to
    /// <param name="max"></param> the maximum value you want the angle to clamp to
    /// <returns></returns> returns the new angle, clamped if outside of the min and max bounds
    private float ClampAngle(float angle, float min, float max)
    {
        float centralAngle = (min + max) / 2;

        if (angle < centralAngle - 360f)
        {
            angle += 360f;
        }

        if (angle > centralAngle + 360f)
        {
            angle -= 360f;
        }

        return Mathf.Clamp(angle, min, max);
    }

    ///////////////////////////////// MOVEMENT-RELATED METHODS /////////////////////////////////////////

    /// <summary>
    /// Checks if the player is on the ground using a Physics raycast of a sphere
    /// </summary>
    private void GroundedCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);

        //PLACEHOLDER - USED FOR ANIMATOR, edge cases with falling so doing it the dumb dumb way
        //playerAnimator.SetBool("isJumping", false);
    }

    /// <summary>
    /// Jumps (called on input) if the player is grounded and not on the jump cooldown by adding a force to the rigidbody
    /// </summary>
    private void Jump()
    {
        if (!canJump || !isGrounded)
        {
            return;
        }

        rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);

        //PLACEHOLDER - USED FOR ANIMATOR
        //playerAnimator.SetBool("isJumping", true);

        canJump = false;
        StartCoroutine(JumpCooldown());
    }

    /// <summary>
    /// Adds extra gravity to shape the jump arc so that falls are faster than rises,
    /// and releasing jump while rising cuts the jump short.
    /// </summary>
    private void ApplyJumpGravity()
    {
        float verticalSpeed = rb.linearVelocity.y;
        float multiplier = 1f;

        if (verticalSpeed < 0f)
        {
            multiplier = fallGravityMultiplier;
        }
        else if (verticalSpeed > 0f && !isJumpHeld)
        {
            multiplier = lowJumpGravityMultiplier;
        }

        // Rigidbody already applies 1x gravity, so only add the difference
        rb.linearVelocity += Vector3.up * Physics.gravity.y * (multiplier - 1f) * Time.fixedDeltaTime;

        // adds drag to the Y value to make jumping look more natural
        if (!isGrounded && !MovementLocked)
        {
            Vector3 v = rb.linearVelocity;
            v.y *= 1f - dragForce * Time.fixedDeltaTime;
            rb.linearVelocity = v;
        }
    }

    /// <summary>
    /// The coroutine that runs the duration of the jump cooldown. Waits 0.25s by default, and until grounded if not grounded by that time.
    /// Once grounded, waits the duration of jumpCooldown before allowing the player to jump again
    /// </summary>
    /// <returns>
    /// An <see cref="System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    private IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(0.25f);

        var waitForGrounded = new WaitUntil(() => isGrounded);
        yield return waitForGrounded;

        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    /// <summary>
    /// Moves the target at an accelerating speed (for smoothness) up to a default maximum, multiplied by a sprint multiplier if the player is currently sprinting.
    /// Movement is by default in the direction of WASD with W always pointing in the direction the player is looking, and ASD relative to that.
    /// </summary>
    private void Move()
    {
        float targetSpeed = (isRunning ? moveSpeed * sprintMultiplier : moveSpeed) * move.magnitude;

        playerAnimator.speed = (isRunning ? animatorRunningSpeed : animatorWalkingSpeed);

        // Snap to zero when there's no input, otherwise ease toward the target speed. NECESSARY FOR ANIMATOR TO WORK
        if (move.sqrMagnitude < 0.01f)
        {
            currentSpeed = 0f;
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.fixedDeltaTime * acceleration);
        }

        //ORIGINALcurrentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.fixedDeltaTime * acceleration);

        // Rotate the player body to match the camera's horizontal rotation (mouse X)
        rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));

        Vector3 forward = cameraTarget.forward;
        Vector3 right = cameraTarget.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * move.y + right * move.x).normalized;
        Vector3 currentVelocity = rb.linearVelocity;

        // While locked, something else (a dash for now) owns horizontal velocity
        if (!MovementLocked)
        {
            if (moveDirection.sqrMagnitude > 0.01f)
            {
                Vector3 horizontalVelocity = moveDirection * currentSpeed;

                // Look ahead for a wall so we don't keep pushing into it
                float checkDistance = wallCheckDistance + horizontalVelocity.magnitude * Time.fixedDeltaTime;

                if (rb.SweepTest(moveDirection, out RaycastHit hit, checkDistance, QueryTriggerInteraction.Ignore)
                    && hit.normal.y < maxWalkableNormalY)
                {
                    // Remove the part of our movement that goes into the wall, keep the part along it
                    Vector3 wallNormal = new Vector3(hit.normal.x, 0f, hit.normal.z).normalized;
                    horizontalVelocity = Vector3.ProjectOnPlane(horizontalVelocity, wallNormal);
                }

                rb.linearVelocity = new Vector3(horizontalVelocity.x, currentVelocity.y, horizontalVelocity.z);
            }
            else
            {
                rb.linearVelocity = new Vector3(0f, currentVelocity.y, 0f);
            }
        }

        //USED FOR ANIMATOR, takes flat velocity ignoring acceleration, used for anim triggers
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        playerAnimator.SetFloat("playerSpeed", flatVelocity.magnitude);
    }

    /// <summary>
    /// Rotates the camera around the player based on the displacement of the mouse's position and the lookSensitivity.
    /// </summary>
    private void Look()
    {
        if (look.sqrMagnitude >= lookThreshold)
        {
            yaw += look.x * lookSensitivity;
            pitch -= look.y * lookSensitivity;
        }

        yaw = ClampAngle(yaw, float.MinValue, float.MaxValue);
        pitch = ClampAngle(pitch, bottomClampAngle, topClampAngle);

        cameraTarget.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    //////////////// DASH-RELATED METHODS ////////////////

    /// <summary>
    /// applies velocity to make player dash
    /// </summary>
    private void Dash()
    {
        if (!canDash)
        {
            return;
        }
        rb.linearDamping = 0;

        Transform forwardT;
        forwardT = orientation;

        Vector3 direction = GetDirection(forwardT);

        moveSpeed += dashSpeedIncrease;
        MovementLocked = true;
        Vector3 forceToApply = direction * dashPower + orientation.up * dashUpwardPower;

        if (disableGravity)
        {
            rb.useGravity = false;
        }

        if (resetVel)
        {
            rb.linearVelocity = Vector3.zero;
        }

        rb.AddForce(forceToApply, ForceMode.Impulse);
        Invoke(nameof(ResetDash), dashDuration);

        canDash = false;
        StartCoroutine(DashCooldown());
    }

    /// <summary>
    /// Helper method to stop the dash after set time
    /// </summary>
    private void ResetDash()
    {
        MovementLocked = false;
        moveSpeed -= dashSpeedIncrease;

        if (disableGravity)
        {
            rb.useGravity = true;
        }
    }

    /// <summary>
    /// The coroutine that runs the duration of the dash cooldown. Waits 0.25s by default, then
    /// waits the duration of dashCooldown before allowing the player to dash again
    /// </summary>
    /// <returns>
    /// An <see cref="System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashDuration);

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    /// <summary>
    /// helper method to determine the direction of the dash
    /// </summary>
    /// <param name="forwardT"> Transform being used to extract the direction </param>
    /// <returns> A vector of the direction the player should dash </returns>
    private Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3();
        if (allowAllDirections)
        {
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        }
        else
        {
            direction = forwardT.forward;
        }
        if (verticalInput == 0 && horizontalInput == 0)
        {
            direction = forwardT.forward;
        }
        return direction.normalized;
    }

    ///////////////////////////////// ON METHODS FOR RECOGNIZING UNITY'S INPUT SYSTEM /////////////////////////////////////

    /// <summary>
    /// Retrieves the input from the Move input action in Unity's InputSystem
    /// </summary>
    /// <param name="inputValue"></param> the value of the Move input action (a Vector2)
    private void OnMove(InputValue inputValue)
    {
        move = inputValue.Get<Vector2>();
    }

    /// <summary>
    /// Retrieves the input from the Jump input action in Unity's InputSystem
    /// </summary>
    /// <param name="inputValue"></param> the value of the Jump input action (a button press or release)
    private void OnJump(InputValue inputValue)
    {
        isJumpHeld = inputValue.isPressed;

        if (inputValue.isPressed)
        {
            Jump();
        }
    }

    /// <summary>
    /// Retrieves the input from the Sprint input action in Unity's InputSystem
    /// </summary>
    /// <param name="inputValue"></param> the value of the Sprint input action (a button press or release)
    private void OnSprint(InputValue inputValue)
    {
        isRunning = inputValue.isPressed;
    }

    /// <summary>
    /// Retrieves the input from the Look input action in Unity's InputSystem
    /// </summary>
    /// <param name="inputValue"></param> the value of the Look input action (a mouse's displacement/delta)
    private void OnLook(InputValue inputValue)
    {
        look = inputValue.Get<Vector2>();
    }

    /// <summary>
    /// Retrieves the input from the Dash input action in Unity's InputSystem
    /// </summary>
    /// <param name="inputValue"></param> the value of the Dash input action (a button press or release)
    private void OnDash(InputValue inputValue)
    {
        isDashHeld = inputValue.isPressed;

        if (inputValue.isPressed)
        {
            Dash();
        }
    }
}
