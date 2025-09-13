using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LookingAround : State<Stalker>
{
    private float speedBeforeEnteringLookAroundState = 0.0f;
    private Vector3 noisePosition;

    public void Enter(Stalker stalker)
    {
        stalker.currentStalkerState = "LookingAround";
        stalker.animator.SetTrigger("ArrievedAtNoicePosition");
        speedBeforeEnteringLookAroundState = stalker.agentMovement.speed;
        stalker.agentMovement.speed = 0.0f;
        noisePosition = stalker.noice.position;
        stalker.animator.SetFloat("LookAroundAnimationIndex",(float) NoiseBroker.Instance.GetLookAroundAnimationIndex(stalker.noice.position, stalker));
        stalker.ChooseRandomSfx(new string[] { "Clicking", "Roaring", "Laughing", "Growl" });

    }
    public void Update(Stalker stalker)
    {


        if (MessageBroker.Instance.IsEngagement() && Vector3.Distance(stalker.transform.position, stalker.player.position) < stalker.startToChaseDistanceWhenEngaged)
            stalker.StartEngageToPlayer();
    }
    public void Exit(Stalker stalker)
    {
        stalker.previousStalkerState = "LookingAround";
        stalker.animator.ResetTrigger("ArrievedAtNoicePosition");
        stalker.agentMovement.speed = speedBeforeEnteringLookAroundState;

        NoiseBroker.Instance.RemoveStalkerFromInspectingNoiseOrigin(noisePosition, stalker);

        stalker.audioManager.StopAllSounds();
    }


}
