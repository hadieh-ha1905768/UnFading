using System.Collections;
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

    //movement variables
    [SerializeField] internal Vector2 currentMovementInput;
    [SerializeField] internal Vector3 currentMovement;
    [SerializeField] internal Vector3 currentRunMovement;
    [SerializeField] internal bool isMovementPressed;
    [SerializeField] internal float rotationFactorPerFrame = 15.0f;

    //running variables
    [SerializeField] internal bool isRunPressed;
    [SerializeField] internal float runMultiplier = 3.0f;

    //jumping variables
    [SerializeField] internal bool isJumpPressed = false;
    [SerializeField] internal float initialJumpVelocity;
    [SerializeField] internal float maxJumpHeight = 2f;
    [SerializeField] internal float maxJumpTime = 0.75f;
    [SerializeField] internal bool isJumping = false;

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

    private void setupJumpVariables(){
        float timeToApex = maxJumpTime / 2.0f;
        gravity = (-2 * maxJumpHeight)/Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
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

    private void onJump(InputAction.CallbackContext context){
        isJumpPressed = context.ReadValueAsButton();
    }

    private void Update()
    {
        handleRotation();
        if (isRunPressed)
        {
            characterController.Move(currentRunMovement * Time.deltaTime);
        }
        else
        {
            characterController.Move(currentMovement * Time.deltaTime);
        }
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
            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        }
        else if(isFalling){
            float prevYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            float nextYVelocity = Mathf.Max((prevYVelocity + newYVelocity) * 0.5f, -20.0f);
            currentMovement.y = nextYVelocity;
            currentRunMovement.y = nextYVelocity;
        }
        else
        {
            float prevYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * Time.deltaTime);
            float nextYVelocity = (prevYVelocity + newYVelocity) * 0.5f;
            currentMovement.y = nextYVelocity;
            currentRunMovement.y = nextYVelocity;
        }
    }

    private void handleJump(){
        if(!isJumping && characterController.isGrounded && isJumpPressed){
            isJumping = true;
            currentMovement.y = initialJumpVelocity * 0.5f;
            currentRunMovement.y = initialJumpVelocity * 0.5f;
        }
        else if(isJumping && characterController.isGrounded && !isJumpPressed){
            isJumping = false;
        }
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