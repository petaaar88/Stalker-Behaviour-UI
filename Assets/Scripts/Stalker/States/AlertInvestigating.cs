using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AlertInvestigating : State<Stalker>
{
    private bool isArrivedAtNoiseOriginPosition = false; // TODO: rename
    private Vector3 noiseOriginPosition;
    private string sfxName;

    public void Enter(Stalker stalker)
    {
        stalker.currentStalkerState = "AlertInvestigating";
        stalker.agentMovement.speed = stalker.relocatingSpeed;

        
        if (stalker.stateMachine.GetPreviousState() == stalker.stateMachine.lookingAroundState)
            stalker.animator.SetTrigger("InvestigationEnd");
        else
            stalker.animator.SetTrigger("ExitCover");

        NoiseBroker.Instance.AddStalkerToInspectNoiseOrigin(stalker.noice.position, stalker);
        noiseOriginPosition = stalker.noice.position;
        isArrivedAtNoiseOriginPosition = false;

        stalker.investigationNoise.position = NoiseBroker.Instance.GetLandingPosition(stalker.noice.position, stalker);
        stalker.agentMovement.SetTarget(stalker.investigationNoise);
        stalker.audioManager.PlaySound("RunningToCover");
        sfxName = stalker.ChooseRandomSfx(new string[] {  "Growl","Growl2" });
    }

    public void Update(Stalker stalker)
    {
        if (MessageBroker.Instance.IsEngagement() && Vector3.Distance(stalker.transform.position, stalker.player.position) < stalker.startToChaseDistanceWhenEngaged)
            stalker.StartEngageToPlayer();

        if (Vector3.Distance(stalker.agentMovement.pathSolver.grid.NodeFromWorldPoint(stalker.investigationNoise.position).worldPosition, stalker.transform.position) <= stalker.agentMovement.stoppingDistance)
        {
            stalker.previousLoudSubtlePosition = NoiceListener.Instance.subtleNoicePosition;
            isArrivedAtNoiseOriginPosition = true;
            stalker.stateMachine.ChangeState(stalker.stateMachine.lookingAroundState);
        }
    }

    public void Exit(Stalker stalker)
    {
        stalker.previousStalkerState = "AlertInvestigating";
        stalker.animator.ResetTrigger("InvestigationEnd");
       
        stalker.animator.ResetTrigger("ExitCover");

        if (!isArrivedAtNoiseOriginPosition)
            NoiseBroker.Instance.RemoveStalkerFromInspectingNoiseOrigin(noiseOriginPosition, stalker);

        stalker.audioManager.StopSound("RunningToCover");

        if (stalker.audioManager.IsSoundPlaying(sfxName))
            stalker.audioManager.StopSound(sfxName);
    }
}
