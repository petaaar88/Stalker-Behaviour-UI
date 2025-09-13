
using UnityEngine;

public class Recovering : State<Stalker>
{

    public void Enter(Stalker stalker)
    {
        stalker.canChasePlayer = false;
        stalker.currentStalkerState = "Recovering";
        stalker.agentMovement.SetTarget(stalker.coversPositions[stalker.currentCoverIndex].gameObject.transform);
        stalker.agentMovement.speed = stalker.relocatingSpeed;
        stalker.animator.SetTrigger("ChaseEnd");

        stalker.animator.applyRootMotion = false;
        stalker.audioManager.PlaySound("RunningToCover");
        stalker.audioManager.PlaySound("Mumble");
    }

    public void Update(Stalker stalker)
    {
        if (Vector3.Distance(stalker.coversPositions[stalker.currentCoverIndex].gameObject.transform.position, stalker.transform.position) <= stalker.agentMovement.stoppingDistance)
            stalker.stateMachine.ChangeState(stalker.stateMachine.inCoverState);
    }

    public void Exit(Stalker stalker)
    {
        stalker.animator.applyRootMotion = true;

        stalker.canChasePlayer = true;
        stalker.previousStalkerState = "Recovering";
        stalker.previousLoudSubtlePosition = NoiceListener.Instance.subtleNoicePosition;
        stalker.previousLoudNoicePosition = NoiceListener.Instance.loudNoicePosition;
        stalker.audioManager.StopSound("RunningToCover");
        stalker.audioManager.StopSound("Mumble");
    }
}