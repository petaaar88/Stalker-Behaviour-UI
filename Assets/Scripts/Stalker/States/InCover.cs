using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class InCover : State<Stalker>
{
    private float coverTimer = 0f;
    private bool isTimerStarted = false;
    private bool isMovingToCover = false;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float startRotationY;
    private float targetRotationY;
    private float moveTimer = 0f;
    private float moveDuration = 0.3f; 
    private Collider coverWallCollider;

    public void Enter(Stalker stalker)
    {
        coverWallCollider = stalker.coversPositions[stalker.currentCoverIndex].coverCollider;

        if (stalker.gameObject.GetComponent<CapsuleCollider>() != null && coverWallCollider != null)
        {
            Physics.IgnoreCollision(stalker.gameObject.GetComponent<CapsuleCollider>(), coverWallCollider, true);
        }
        stalker.animator.SetTrigger("EnterCover");
        coverTimer = 0f;
        isTimerStarted = false; 
        isMovingToCover = true;
        stalker.currentStalkerState = "InCover";
        stalker.agentMovement.speed = 0.0f;
        stalker.animator.SetBool("isCrouchIdleMirror", stalker.coversPositions[stalker.currentCoverIndex].mirrored);

        startPosition = stalker.gameObject.transform.position;
        targetPosition = stalker.coversPositions[stalker.currentCoverIndex].position;
        
        targetPosition.y = startPosition.y;

       
        startRotationY = stalker.gameObject.transform.eulerAngles.y;
        targetRotationY = stalker.coversPositions[stalker.currentCoverIndex].rotation;

        moveTimer = 0f;
        stalker.agentMovement.Disable();
    }

    public void Update(Stalker stalker)
    {
        if (isMovingToCover)
        {
            moveTimer += Time.deltaTime;
            float moveProgress = Mathf.Clamp01(moveTimer / moveDuration);

            float smoothProgress = Mathf.SmoothStep(0f, 1f, moveProgress);
            
            Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, smoothProgress);
            stalker.gameObject.transform.position = currentPos;

            Vector3 currentRotation = stalker.gameObject.transform.eulerAngles;
            currentRotation.y = Mathf.LerpAngle(startRotationY, targetRotationY, smoothProgress);
            stalker.gameObject.transform.eulerAngles = currentRotation;

            if (moveProgress >= 1f)
            {
                isMovingToCover = false;
                isTimerStarted = true; // Now start the cover timer
                moveTimer = 0f;
            }
        }
        else if (isTimerStarted)
        {
            coverTimer += Time.deltaTime;
            if (coverTimer >= stalker.secondsInCover)
            {
                isTimerStarted = false;
                stalker.currentCoverIndex = (stalker.currentCoverIndex + 1) % stalker.coversPositions.Count;
                stalker.stateMachine.ChangeState(stalker.stateMachine.relocatingState);
            }
        }
    }

    public void Exit(Stalker stalker)
    {
        isTimerStarted = false;
        isMovingToCover = false;
        coverTimer = 0f;
        moveTimer = 0f;
        stalker.previousStalkerState = "InCover";
        stalker.animator.ResetTrigger("EnterCover");
        stalker.animator.SetTrigger("ExitCover");
        stalker.agentMovement.Enable();

        if (stalker.gameObject.GetComponent<CapsuleCollider>() != null && coverWallCollider != null)
        {
            Physics.IgnoreCollision(stalker.gameObject.GetComponent<CapsuleCollider>(), coverWallCollider, false);
        }

    }
}