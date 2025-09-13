using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
    private bool isCrouch;
    private Animator animator;
    private vThirdPersonController playerController;
    private float standingWalkSpeed;
    private float standingSpringSpeed;
    private float crouchWalkSpeed;
    private float crouchSpringSpeed;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<vThirdPersonController>();

        standingWalkSpeed = playerController.freeSpeed.walkSpeed;
        standingSpringSpeed = playerController.freeSpeed.sprintSpeed;
        crouchWalkSpeed = playerController.freeSpeed.walkSpeed == 1 ? 1 : playerController.freeSpeed.walkSpeed - 4;
        crouchSpringSpeed = playerController.freeSpeed.sprintSpeed == 2 ? 2 : playerController.freeSpeed.sprintSpeed - 4;

    }

    // Update is called once per frame
    void Update()
    {
        if (isCrouch)
        {
            playerController.freeSpeed.walkSpeed = crouchWalkSpeed;
            playerController.freeSpeed.sprintSpeed = crouchSpringSpeed;

        }
        else
        {
            playerController.freeSpeed.walkSpeed = standingWalkSpeed;
            playerController.freeSpeed.sprintSpeed = standingSpringSpeed;

        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouch = !isCrouch;

            animator.SetBool("IsCrouch", isCrouch);
        }
    }
}
