using System.Collections;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Player player;

    [SerializeField] internal Animator animator;

    int isWalkingHash;
    int isRunningHash;
    int isJumpingHash;
    int jumpCountHash;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");
        jumpCountHash = Animator.StringToHash("jumpCount");
    }

    private void handleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        if (player.playerMovement.isMovementPressed && !isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        }
        else if (!player.playerMovement.isMovementPressed & isWalking)
        {
            animator.SetBool(isWalkingHash, false);
        }

        if((player.playerMovement.isMovementPressed && player.playerMovement.isRunPressed) && !isRunning){
            animator.SetBool(isRunningHash, true);
        }
        else if((!player.playerMovement.isMovementPressed || !player.playerMovement.isRunPressed) && isRunning){
            animator.SetBool(isRunningHash, false);
        }

    }

    public void handleJumpAnimation(bool isJumping){
        animator.SetBool(isJumpingHash, isJumping);
    }

    public void handleJumpCountAnimation(int count){
        animator.SetInteger(jumpCountHash, count);
    }

    private void Update()
    {
        handleAnimation();
    }
}
