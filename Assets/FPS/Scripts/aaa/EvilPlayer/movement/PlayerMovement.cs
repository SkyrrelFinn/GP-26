using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction sprintAction;
    private CharacterController controller;
    private Vector3 gravityVelocity, velocity;
    private bool jumping, checkCeiling;
    private static bool sprinting; public static bool Sprinting => sprinting;    
    private float moveSpeed;
    private static Vector3 distanceVelocity;
    public static Vector3 DistanceVelocity { get { return distanceVelocity; } }
    private static Vector2 moveInput; public static Vector2 MoveInput => moveInput;
    private float activeFriction = 10f;    

    [SerializeField] 
    private float walkSpeed = 5f, sprintSpeed = 8f, acceleration = 15f, defaultFriction = 15f, airControl = .5f;
    [SerializeField] private bool alwaysSprint;
    [SerializeField] public static readonly float gravity = -25f;
    [SerializeField] private Transform orientation, ceilingCheck, groundCheck;    
    [SerializeField] private float groundCheckRadius = 0.4f;
    [SerializeField] private LayerMask ignoredGroundLayers;    
    private float slopeAngle;
    private bool touchingSlope;
    ControllerColliderHit slopeHit;
    private float _airControl;
    float stopSpeed = 1f;
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        activeFriction = defaultFriction;
        inputActions = GetComponent<PlayerInput>().actions;
        moveAction = inputActions.FindAction("Move");
        sprintAction = inputActions.FindAction("Sprint");

        sprintAction.performed += OnSprintStarted;
        sprintAction.canceled += OnSprintCanceled;
    }
    void OnDisable()
    {
        Debug.Log("ondisable");
        sprintAction.performed -= OnSprintStarted;
        sprintAction.canceled -= OnSprintCanceled;
    }
    #region Update
    void Update()
    {
        if (GameManager.Paused) return;
        if (DialogueManager.GetInstance() != null && DialogueManager.GetInstance().dialogueIsPlaying) return;

        ApplyFriction();
        MovePlayer();        
        CheckForCeilingCollisions();
        ApplyGravity();

        if (IsGrounded()) _airControl = 1;
        else _airControl = airControl;
        if(sprinting && PlayerStamina.exhaustion) sprinting = false;        
    }
    private void MovePlayer()
    {        
        moveInput = moveAction.ReadValue<Vector2>();
        moveSpeed = GetMoveSpeed();

        if (IsGrounded()) GroundAcceleration();
        else AirAcceleration();

        controller.Move(velocity * Time.deltaTime);
        distanceVelocity = controller.velocity;
    }

    private float GetMoveSpeed()
    {
        if(!IsGrounded()) return Mathf.Max(moveSpeed, walkSpeed);
        if (sprinting) return sprintSpeed;

        return walkSpeed;
    }
    #endregion
    #region Velocity handling
    private void GroundAcceleration()
    {
        Vector3 wishDir = (orientation.forward * moveInput.y + orientation.right * moveInput.x).normalized;
        float curSpeed = Vector3.Dot(velocity, wishDir);
        float addSpeed = moveSpeed - curSpeed;
        if (addSpeed <= 0f) return;

        float accelSpeed = acceleration * moveSpeed * Time.deltaTime;
        if (accelSpeed > addSpeed) accelSpeed = addSpeed;
        velocity += wishDir * accelSpeed;
    }
    private void AirAcceleration()
    {
        Vector3 wishDir = (orientation.forward * moveInput.y + orientation.right * moveInput.x).normalized;

        float curSpeed = Vector3.Dot(velocity, wishDir);
        float addSpeed = moveSpeed - curSpeed;
        if (addSpeed <= 0f) return;

        Vector3 horizontalVel = velocity;
        horizontalVel.y = 0f;
        float speed = horizontalVel.magnitude;

        float accel = acceleration * _airControl * Time.deltaTime;
        if (speed > moveSpeed)
            accel *= moveSpeed / speed;
        float accelSpeed = Mathf.Min(accel * moveSpeed, addSpeed);
        velocity += wishDir * accelSpeed;
    }
    public void AddForce(Vector3 f)
    {
        velocity += f;
    }
    #endregion
    #region Gravity & Friction
    private void ApplyGravity()
    {
        if (IsGrounded() && !OnSlope() && !jumping)
            gravityVelocity.y = -3;
        else
            gravityVelocity.y += gravity * Time.deltaTime;

        Vector3 gravityDir = gravityVelocity.normalized;

        if (OnSlope())
            gravityDir = GetSlopeMoveDirection(gravityVelocity).normalized;

        controller.Move(gravityDir * gravityVelocity.magnitude * Time.deltaTime);
    }
    private void ApplyFriction()
    {
        if (!IsGrounded())
            return;

        Vector3 vel = velocity;
        vel.y = 0f; // horizontal only

        float speed = vel.magnitude;
        if (speed < 0.1f)
            return;

        float control = speed < stopSpeed ? stopSpeed : speed;
        float drop = control * activeFriction * Time.deltaTime;

        float newSpeed = speed - drop;
        if (newSpeed < 0f)
            newSpeed = 0f;

        velocity.x *= newSpeed / speed;
        velocity.z *= newSpeed / speed;
    }
    #endregion
    #region Sprinting
    private void OnSprintStarted(InputAction.CallbackContext context)
    {
        if(PlayerStamina.exhaustion) return;
        sprinting = true;
    }
    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        if(alwaysSprint)
        {
            sprinting = true;
            return;
        }
        sprinting = false;
    }
    #endregion
    #region Jumping    
    public void Jump(float height)
    {
        gravityVelocity.y += height;
        checkCeiling = true;
        StartCoroutine(JumpCoroutine(0.1f));
    }
    private IEnumerator JumpCoroutine(float dur)
    {
        jumping = true;
        controller.stepOffset = 0.01f;

        yield return new WaitForSeconds(dur);
        yield return new WaitUntil(() => gravityVelocity.y <= 0);

        jumping = false;
        controller.stepOffset = 0.3f;
    }
    #endregion
        #region Collisions & Slopes
    private void CheckForCeilingCollisions()
    {
        if (!checkCeiling) return;

        if (Physics.CheckSphere(ceilingCheck.position, controller.radius, ~ignoredGroundLayers))
        {
            gravityVelocity.y = 0;      
            jumping = false;   
            checkCeiling = false;
            return;
        }
        if (IsGrounded())
            checkCeiling = false;
    }
    public bool IsGrounded(float checkRadius = 0.4f)
    {
        if (jumping) return false;

        if (checkRadius == 0.4f)
            return Physics.CheckSphere(groundCheck.position, groundCheckRadius, ~ignoredGroundLayers);
        else
            return Physics.CheckSphere(groundCheck.position, checkRadius, ~ignoredGroundLayers);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        SlopeHit(hit);
    }

    private void SlopeHit(ControllerColliderHit hit)
    {
        slopeHit = hit;
        if (hit.point.y > transform.position.y + controller.radius)
        {
            touchingSlope = false;
            return;
        }
        float angle = Vector3.Angle(hit.normal, Vector3.up);
        if (angle > controller.slopeLimit)
        {
            slopeAngle = angle;
            touchingSlope = true;
        }
        else
        {
            touchingSlope = false;
        }
    }

    public bool OnSlope()
    {
        RaycastHit hit;
        if (!IsGrounded())
            return false;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, transform.localScale.y + 0.1f, ~ignoredGroundLayers))
        {
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            if (angle <= controller.slopeLimit)
                return false;
        }
        return touchingSlope;
    }
    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
    public bool IsStandingOn(GameObject target)
    {
        if (controller == null || target == null) return false;

        Vector3 centerWorld = transform.TransformPoint(controller.center);
        float halfHeight = controller.height / 2f - controller.radius;

        Vector3 point1 = centerWorld + Vector3.up * halfHeight;
        Vector3 point2 = centerWorld - Vector3.up * halfHeight;

        float castDistance = 0.2f;

        if (Physics.CapsuleCast(point1, point2, controller.radius, Vector3.down, out RaycastHit hit, castDistance))
        {
            return hit.collider.gameObject == target;
        }

        return false;
    }
    #endregion
}
