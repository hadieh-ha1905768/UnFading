using System.Collections;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Player player;

    [SerializeField] internal Animator animator;

    int isWalkingHash;
    int isRunningHash;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
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

    private void Update()
    {
        handleAnimation();
    }
}
