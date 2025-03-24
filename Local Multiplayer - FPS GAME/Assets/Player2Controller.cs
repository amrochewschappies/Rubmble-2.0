using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player2Controller : MonoBehaviour
{
    
    
    #region Player Control

    [Header("Movement settings")]
    //move
    [SerializeField] [Range(0f, 15f)] private float moveSpeed = 0f;

    //jump
    [SerializeField] [Range(0f, 20f)] private float jumpForce = 5f;
    private float gravityMultiplier = 3f;
    private float lowJumpMultiplier = 4f;
    private float coyoteTime = 0.1f;
    private float jumpBufferTime = 0.2f;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    //sensitivity
    [SerializeField] [Range(0f, 15f)] private float mouseSensitivity = 2f;
    [SerializeField] [Range(0f, 25f)] private float controllerSensitivityX = 15f;
    [SerializeField] [Range(0f, 25f)] private float controllerSensitivityY = 15f;

    //deadzone
    [SerializeField] [Range(0f, 1f)] private float controllerDeadzone = 0.5f;

    #endregion

    #region ControlScheme checks

    private bool isMouse;
    private bool isController;

    #endregion

    #region Refs

    [Header("References")] public Transform cameraTransform;
    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 moveTile;
    private bool jumpInput;
    private PlayerInput playerInput;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool wasInAir = false;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public Animator PlayerAnimator;
    #endregion

    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        // Determine if the input device is a mouse or controller
        isMouse = playerInput.currentControlScheme == "Keyboard&Mouse";
        isController = playerInput.currentControlScheme == "Gamepad";

        // Handle player movement
        playerInput.actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Move"].canceled += ctx => moveInput = Vector2.zero;

        // Handle player look
        playerInput.actions["Look"].performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Look"].canceled += ctx => lookInput = Vector2.zero;

        // Handle player jump
        playerInput.actions["Jump"].performed += ctx => jumpInput = true;
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
        HandleLook();
        ApplyGravity();
    }

    // Use LateUpdate() to process the camera movement after player has moved
    private void LateUpdate()
    {
    }

    [ContextMenu("HandleMovement")]
    private void HandleMovement()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move *= moveSpeed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        bool isRunning = moveInput.magnitude > 0; // If there is movement input
        PlayerAnimator.SetBool("IsRunning", isRunning);

        if (isRunning)
        {
            PlayerAnimator.SetBool("IsLifting", false);
            PlayerAnimator.SetBool("IsLowering", false);
        }
    }

    private float xRotation = 0f;
    private void HandleLook()
    {
        if (isMouse)
        {
            HandleMouseLook();
        }
        else if (isController)
        {
            HandleControllerLook();
        }
    }

    private void HandleMouseLook()
    {
        float lookX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float lookY = lookInput.y * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * lookX);

        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void HandleControllerLook()
    {
        float controllerLookX = lookInput.x * controllerSensitivityX;
        float controllerLookY = lookInput.y * controllerSensitivityY;
        
        if (Mathf.Abs(lookInput.x) < controllerDeadzone) controllerLookX = 0;
        if (Mathf.Abs(lookInput.y) < controllerDeadzone) controllerLookY = 0;

        transform.Rotate(Vector3.up * controllerLookX);

        xRotation -= controllerLookY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void HandleJump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);

        if (isGrounded)
        {
            if (wasInAir)
            {
                StartCoroutine(TriggerRumble(0.2f, 0.8f, 0.175f)); 
            }
            coyoteTimeCounter = coyoteTime;
            wasInAir = false;
            PlayerAnimator.SetBool("IsJumping", false);
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            wasInAir = true;
        }

        if (jumpInput)
        {
            PlayerAnimator.SetBool("IsJumping", true);
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            jumpBufferCounter = 0;
        }

        jumpInput = false;
    }

    public void ApplyGravity()
    {
        if (!isGrounded)
        {
            if (rb.linearVelocity.y > 0 && !jumpInput)
            {
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
            else
            {
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (gravityMultiplier - 1) * Time.deltaTime;
            }
        }
    }

    private IEnumerator TriggerRumble(float lowFrequency, float highFrequency, float duration)
    {
        if (isController)
        {
            Gamepad gamepad = Gamepad.current;
            if (gamepad != null)
            {
                gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
                yield return new WaitForSeconds(duration);
                gamepad.SetMotorSpeeds(0, 0);
            }     
        }

    }
}
