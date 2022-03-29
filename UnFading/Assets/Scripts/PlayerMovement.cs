using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //player script
    [SerializeField] private Player player;

    //player components
    [SerializeField] internal CharacterController characterController;

    //gravity variables
    [SerializeField] internal float gravity = -9.8f;
    [SerializeField] internal float groundedGravity = -0.05f;
    float secondGravity;
    float thirdGravity;
    float initialJumpVelocity;
    float secondInitialJumpVelocity;
    float thirdInitialJumpVelocity;

    //movement variables
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    Vector3 appliedMovement;
    internal bool isMovementPressed;
    float rotationFactorPerFrame = 15.0f;

    //running variables
    internal bool isRunPressed;
    [SerializeField] internal float runMultiplier = 3.0f;

    //jumping variables
    bool isJumpPressed = false;
    bool isJumping = false;
    [SerializeField] internal float maxJumpHeight = 2f;
    [SerializeField] internal float maxJumpTime = 0.75f;
    [SerializeField] internal int jumpCount = 0;
    Dictionary<int, float> initialJumpVelocities = new Dictionary<int, float>();
    Dictionary<int, float> jumpGravities = new Dictionary<int, float>();
    Coroutine currentJumpResetRoutine = null;

    //animation flags
    bool isJumpAnimating = false;


    private void Awake()
    {
        player.playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();

        player.playerInput.CharacterControls.Move.started += onMovementInput;
        player.playerInput.CharacterControls.Move.canceled += onMovementInput;
        player.playerInput.CharacterControls.Move.performed += onMovementInput;
        player.playerInput.CharacterControls.Run.started += onRun;
        player.playerInput.CharacterControls.Run.canceled += onRun;
        player.playerInput.CharacterControls.Jump.started += onJump;
        player.playerInput.CharacterControls.Jump.canceled += onJump;

        setupJumpVariables();
    }

    private void setupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2.0f;

        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
        float secondGravity = (-2 * maxJumpHeight + 2) / Mathf.Pow(timeToApex * 1.25f, 2);
        float secondInitialJumpVelocity = (2 * maxJumpHeight + 2) / (timeToApex * 1.25f);
        float thirdGravity = (-2 * maxJumpHeight + 4) / Mathf.Pow(timeToApex * 1.5f, 2);
        float thirdInitialJumpVelocity = (2 * maxJumpHeight + 4) / (timeToApex * 1.5f);

        initialJumpVelocities.Add(1, initialJumpVelocity);
        initialJumpVelocities.Add(2, secondInitialJumpVelocity);
        initialJumpVelocities.Add(3, thirdInitialJumpVelocity);

        jumpGravities.Add(0, gravity);
        jumpGravities.Add(1, gravity);
        jumpGravities.Add(2, secondGravity);
        jumpGravities.Add(3, thirdGravity);
    }

    private void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        currentRunMovement.x = currentMovementInput.x * runMultiplier;
        currentRunMovement.z = currentMovementInput.y * runMultiplier;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    private void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    private void onJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    private void Update()
    {
        handleRotation();
        if (isRunPressed)
        {
            appliedMovement.x = currentRunMovement.x;
            appliedMovement.z = currentRunMovement.z;
        }
        else
        {
            appliedMovement.x = currentMovement.x;
            appliedMovement.z = currentMovement.z;
        }

        characterController.Move(appliedMovement * Time.deltaTime);

        handleGravity();
        handleJump();
    }

    private void handleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    private void handleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f || isJumpPressed;
        float fallMultiplier = 2.0f;

        if (characterController.isGrounded)
        {
            if (isJumpAnimating)
            {
                player.playerAnimation.handleJumpAnimation(false);
                isJumpAnimating = false;
                currentJumpResetRoutine = StartCoroutine(jumpResetRoutine());
                if(jumpCount == 3){
                    jumpCount = 0;
                    player.playerAnimation.handleJumpCountAnimation(jumpCount);
                }
            }
            currentMovement.y = groundedGravity;
            appliedMovement.y = groundedGravity;
        }
        else if (isFalling)
        {
            float prevYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (jumpGravities[jumpCount] * fallMultiplier * Time.deltaTime);
            appliedMovement.y = Mathf.Max((prevYVelocity + currentMovement.y) * 0.5f, -20.0f);
        }
        else
        {
            float prevYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (jumpGravities[jumpCount] * Time.deltaTime);
            appliedMovement.y = (prevYVelocity + currentMovement.y) * 0.5f;
        }
    }

    private void handleJump()
    {
        if (!isJumping && characterController.isGrounded && isJumpPressed)
        {
            if (jumpCount < 3 && currentJumpResetRoutine != null)
            {
                StopCoroutine(currentJumpResetRoutine);
            }
            player.playerAnimation.handleJumpAnimation(true);
            isJumpAnimating = true;
            isJumping = true;
            jumpCount += 1;
            player.playerAnimation.handleJumpCountAnimation(jumpCount);
            currentMovement.y = initialJumpVelocities[jumpCount];
            appliedMovement.y = initialJumpVelocities[jumpCount];
        }
        else if (isJumping && characterController.isGrounded && !isJumpPressed)
        {
            isJumping = false;
        }
    }

    private  IEnumerator jumpResetRoutine(){
        yield return new WaitForSeconds(0.5f);
        jumpCount = 0;
    }

    private void OnEnable()
    {
        player.playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        player.playerInput.CharacterControls.Disable();
    }

}